using System;
using TripNow.Domain.Enums;

namespace TripNow.Domain.Events;

public record ReservationCreated(Guid ReservationId, string CustomerEmail, string TripCountry, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ReservationStatusChanged(Guid ReservationId, ReservationStatus OldStatus, ReservationStatus NewStatus, string? Reason = null) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
