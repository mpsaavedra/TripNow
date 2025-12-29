using System;
using System.Linq;
using FluentAssertions;
using TripNow.Domain.Entities;
using TripNow.Domain.Enums;
using TripNow.Domain.Events;
using Xunit;

namespace TripNow.UnitTests.Domain;

public class ReservationTests
{
    [Fact]
    public void CreateReservation_ShouldInitializeCorrectly_AndRaiseEvent()
    {
        // Arrange
        var email = "test@example.com";
        var country = "US";
        var amount = 100m;

        // Act
        var reservation = new Reservation(email, country, amount);

        // Assert
        reservation.Status.Should().Be(ReservationStatus.PendingRiskCheck);
        reservation.CustomerEmail.Should().Be(email);
        reservation.TripCountry.Should().Be(country);
        reservation.Amount.Should().Be(amount);
        reservation.DomainEvents.Should().ContainSingle(e => e is ReservationCreated);
    }

    [Fact]
    public void Approve_ShouldChangeStatus_AndRaiseEvent()
    {
        // Arrange
        var reservation = new Reservation("test@example.com", "US", 100m);

        // Act
        reservation.Approve();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Approved);
        reservation.DomainEvents.Should().Contain(e => e is ReservationStatusChanged && ((ReservationStatusChanged)e).NewStatus == ReservationStatus.Approved);
    }

    [Fact]
    public void Reject_ShouldChangeStatus_AndRaiseEvent()
    {
        // Arrange
        var reservation = new Reservation("test@example.com", "US", 100m);
        var reason = "High risk";

        // Act
        reservation.Reject(reason);

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Rejected);
        reservation.RiskReason.Should().Be(reason);
        reservation.DomainEvents.Should().Contain(e => e is ReservationStatusChanged && ((ReservationStatusChanged)e).NewStatus == ReservationStatus.Rejected);
    }
}
