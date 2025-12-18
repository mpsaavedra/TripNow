using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TripNow.Application.Features.Reservations.Get;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using Xunit;

namespace TripNow.UnitTests.Application;

public class GetReservationHandlerTests
{
    private readonly Mock<IReservationRepository> _repositoryMock;
    private readonly GetReservationHandler _handler;

    public GetReservationHandlerTests()
    {
        _repositoryMock = new Mock<IReservationRepository>();
        _handler = new GetReservationHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnReservation_WhenFound()
    {
        // Arrange
        var reservation = new Reservation("test@test.com", "US", 100);
        _repositoryMock.Setup(r => r.GetByIdAsync(reservation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        var query = new GetReservation(reservation.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(reservation.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation?)null);

        var query = new GetReservation(id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
