namespace TripNow.Domain.Services;

public class RiskEvaluationRequest
{
    public string CustomerEmail { get; set; } = string.Empty;
    public string TripCountry { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
