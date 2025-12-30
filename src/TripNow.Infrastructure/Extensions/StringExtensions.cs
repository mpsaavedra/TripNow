using System;
using System.Collections.Generic;
using System.Text;
using TripNow.Domain.Enums;

namespace TripNow.Infrastructure.Extensions;

public static class StringExtensions
{
    public static ReservationStatus AsReservationStatus(this string status) =>
        status.Trim().ToUpperInvariant() switch
        {
            "PENDING_RISK_CHECK" => ReservationStatus.PendingRiskCheck,
            "APPROVED" => ReservationStatus.Approved,
            "REJECTED" => ReservationStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unknown status: {status}")
        };
}
