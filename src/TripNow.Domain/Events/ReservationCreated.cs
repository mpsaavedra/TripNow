using System;

namespace TripNow.Domain.Events;

public record ReservationCreated(Guid ReservationId, string CustomerEmail, string TripCountry, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
