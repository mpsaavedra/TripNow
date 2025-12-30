using TripNow.Domain.Enums;

namespace TripNow.Domain.Services;

public class RiskEvaluationBaseResult
{
    public double RiskScore { get; set; }
    public string? RejectionReason { get; set; }
}

public class RiskEvaluationServiceResult : RiskEvaluationBaseResult
{
    public string Status { get; set; } = string.Empty;
}

public class RiskEvaluationResult : RiskEvaluationBaseResult
{
    public ReservationStatus Status { get; set; }
}
