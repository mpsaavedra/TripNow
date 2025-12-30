using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TripNow.Application.Features.Reservations.Create;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using Xunit;

namespace TripNow.UnitTests.Application;

public class CreateReservationHandlerTests
{
    private readonly Mock<IReservationRepository> _repositoryMock;
    private readonly Mock<ICountryRepository> _countryRepositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CreateReservationHandler _handler;

    public CreateReservationHandlerTests()
    {
        _repositoryMock = new Mock<IReservationRepository>();
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new CreateReservationHandler(_repositoryMock.Object, _countryRepositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNewReservation_WhenNoneExists()
    {
        // Arrange
        var command = new CreateReservation("test@test.com", "US", 100);
        _repositoryMock.Setup(r => r.GetByCustomerEmailAndTripAsync(command.CustomerEmail, command.TripCountry, command.Amount, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Status.Should().Be("PendingRiskCheck");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnExisting_WhenDuplicateExists()
    {
        // Arrange
        var command = new CreateReservation("test@test.com", "US", 100);
        var existing = new Reservation(command.CustomerEmail, command.TripCountry, command.Amount);
        
        _repositoryMock.Setup(r => r.GetByCustomerEmailAndTripAsync(command.CustomerEmail, command.TripCountry, command.Amount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existing.Id);
        result.Status.Should().Be("PendingRiskCheck");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
