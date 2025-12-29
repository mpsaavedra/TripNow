using Polly;
using Polly.CircuitBreaker;
using System.Net;
using System.Text;
using System.Text.Json;
using TripNow.Domain.Enums;
using TripNow.Domain.Services;

namespace TripNow.Infrastructure.Services;

public class RiskEvaluationService : IRiskEvaluationService
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public RiskEvaluationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // configure Polly for resiliency
        _policy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (_, _, _, _) =>
                {
                    // TODO: do some logging
                })
            .WrapAsync(
                Policy<HttpResponseMessage>
                    .HandleResult(r => !r.IsSuccessStatusCode)
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 5,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (_, _) =>
                        {
                            // TODO: do some logging
                        },
                        onReset: () =>
                        {
                            // TODO: do some logging
                        }))
            .WrapAsync(
                Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));
    }

    public async Task<RiskEvaluationResult> EvaluateAsync(RiskEvaluationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _policy.ExecuteAsync(() =>
                _httpClient.PostAsync("/risk-evaluation", content, cancellationToken));

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new InvalidOperationException("Risk evaluation service is temporarily unavailable");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Risk evaluation failed: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<RiskEvaluationServiceResult>(responseContent);

            return new RiskEvaluationResult
            {
                RiskScore = result!.RiskScore,
                RejectionReason = result.RejectionReason,
                Status = AsReservationStatus(result.Status),
            };
        }
        catch (BrokenCircuitException)
        {
            throw new InvalidOperationException("Risk evaluation service is temporarily unavailable");
        }
    }

    private ReservationStatus AsReservationStatus(string score) =>
        score.Trim().ToUpperInvariant() switch
        {
            "PENDING_RISK_CHECK" => ReservationStatus.PendingRiskCheck,
            "APPROVED" => ReservationStatus.Approved,
            "REJECTED" => ReservationStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(score), $"Unknown status: {score}")
        };
}
