using TripNow.Domain.Enums;

namespace TripNow.Domain.Events;

public record ReservationStatusChanged(Guid ReservationId, ReservationStatus OldStatus, 
    ReservationStatus NewStatus, string? Reason = null) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
