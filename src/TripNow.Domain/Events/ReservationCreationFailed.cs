using System;
using System.Collections.Generic;
using System.Text;

namespace TripNow.Domain.Events;

public record ReservationCreationFailed(string CustomerEmail, string TripCountry, decimal Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
