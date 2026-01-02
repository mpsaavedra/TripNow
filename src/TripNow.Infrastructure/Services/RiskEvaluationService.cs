using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using System.Net;
using System.Text;
using System.Text.Json;
using TripNow.Domain.Enums;
using TripNow.Domain.Services;
using TripNow.Infrastructure.Extensions;

namespace TripNow.Infrastructure.Services;

public class RiskEvaluationService : IRiskEvaluationService
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly ILogger<RiskEvaluationService> _logger;


    public RiskEvaluationService(HttpClient httpClient, ILogger<RiskEvaluationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        // configure Polly for resiliency
        _policy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning("Delaying for {Delay}ms, then making retry {RetryAttempt}. Error: {Message}", 
                        timespan.TotalMilliseconds, retryAttempt, outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase);
                })
            .WrapAsync(
                Policy<HttpResponseMessage>
                    .HandleResult(r => !r.IsSuccessStatusCode)
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 5,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (outcome, timespan) =>
                        {
                            _logger.LogError("Circuit broken for {Delay}ms due to: {Message}", 
                                timespan.TotalMilliseconds, outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase);
                        },
                        onReset: () =>
                        {
                            _logger.LogInformation("Circuit reset");
                        }))
            .WrapAsync(
                Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));
    }

    public async Task<RiskEvaluationResult> EvaluateAsync(RiskEvaluationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting risk evaluation for customer {CustomerEmail}", request.CustomerEmail);
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _policy.ExecuteAsync(() =>
            {
                _logger.LogDebug("Sending risk evaluation request to external service");
                return _httpClient.PostAsync("/risk-evaluation", content, cancellationToken);
            });

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new InvalidOperationException("Risk evaluation service is temporarily unavailable");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Risk evaluation failed: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<RiskEvaluationServiceResult>(responseContent, _jsonOptions);

            return new RiskEvaluationResult
            {
                RiskScore = result!.RiskScore,
                RejectionReason = result.RejectionReason,
                Status = result.Status.AsReservationStatus(),
            };
        }
        catch (BrokenCircuitException)
        {
            throw new InvalidOperationException("Risk evaluation service is temporarily unavailable");
        }
    }

    
}
