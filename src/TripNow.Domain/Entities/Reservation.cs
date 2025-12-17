using System;
using TripNow.Domain.Enums;
using TripNow.Domain.Events;

namespace TripNow.Domain.Entities;

public class Reservation : TripNow.Domain.Common.BaseEntity
{
    public Guid Id { get; private set; }
    public string CustomerEmail { get; private set; }
    public string TripCountry { get; private set; }
    public decimal Amount { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? RiskReason { get; private set; }

    private Reservation() { }

    public Reservation(string customerEmail, string tripCountry, decimal amount)
    {
        Id = Guid.NewGuid();
        CustomerEmail = customerEmail;
        TripCountry = tripCountry;
        Amount = amount;
        Status = ReservationStatus.PendingRiskCheck;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationCreated(Id, CustomerEmail, TripCountry, Amount));
    }

    public void Approve()
    {
        if (Status != ReservationStatus.PendingRiskCheck) return;
        var oldStatus = Status;
        Status = ReservationStatus.Approved;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ReservationStatusChanged(Id, oldStatus, Status));
    }

    public void Reject(string reason)
    {
        if (Status != ReservationStatus.PendingRiskCheck) return;
        var oldStatus = Status;
        Status = ReservationStatus.Rejected;
        RiskReason = reason;
        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new ReservationStatusChanged(Id, oldStatus, Status, reason));
    }
}
