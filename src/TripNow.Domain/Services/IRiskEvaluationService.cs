using System;
using System.Collections.Generic;
using System.Text;

namespace TripNow.Domain.Services;

public interface IRiskEvaluationService
{
    Task<RiskEvaluationResult> EvaluateAsync(RiskEvaluationRequest request, CancellationToken cancellationToken = default);
}
