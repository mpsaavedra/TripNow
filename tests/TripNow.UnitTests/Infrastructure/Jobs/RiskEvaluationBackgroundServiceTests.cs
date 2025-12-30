using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Moq;
using TripNow.Domain.Entities;
using TripNow.Domain.Enums;
using TripNow.Domain.Interfaces;
using TripNow.Domain.Services;
using TripNow.Infrastructure.Jobs;
using TripNow.Infrastructure.Hubs;
using Xunit;
using FluentAssertions;

namespace TripNow.UnitTests.Infrastructure.Jobs;

public class RiskEvaluationBackgroundServiceTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IRiskEvaluationService> _riskEvaluationServiceMock;
    private readonly Mock<ILogger<RiskEvaluationBackgroundService>> _loggerMock;
    private readonly Mock<IServiceProvider> _scopeServiceProviderMock;
    private readonly Mock<IHubContext<ReservationHub>> _hubContextMock;
    private readonly Mock<IHubClients> _hubClientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly Mock<IRecurringJobManager> _recurringJobManagerMock;

    public RiskEvaluationBackgroundServiceTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _riskEvaluationServiceMock = new Mock<IRiskEvaluationService>();
        _loggerMock = new Mock<ILogger<RiskEvaluationBackgroundService>>();
        _scopeServiceProviderMock = new Mock<IServiceProvider>();
        _hubContextMock = new Mock<IHubContext<ReservationHub>>();
        _hubClientsMock = new Mock<IHubClients>();
        _clientProxyMock = new Mock<IClientProxy>();
        _recurringJobManagerMock = new Mock<IRecurringJobManager>();

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_serviceScopeFactoryMock.Object);

        _serviceScopeFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(_serviceScopeMock.Object);

        _serviceScopeMock
            .Setup(x => x.ServiceProvider)
            .Returns(_scopeServiceProviderMock.Object);

        _scopeServiceProviderMock
            .Setup(x => x.GetService(typeof(IUnitOfWork)))
            .Returns(_unitOfWorkMock.Object);

        _scopeServiceProviderMock
            .Setup(x => x.GetService(typeof(IReservationRepository)))
            .Returns(_reservationRepositoryMock.Object);

        _scopeServiceProviderMock
            .Setup(x => x.GetService(typeof(IRiskEvaluationService)))
            .Returns(_riskEvaluationServiceMock.Object);

        _scopeServiceProviderMock
            .Setup(x => x.GetService(typeof(IHubContext<ReservationHub>)))
            .Returns(_hubContextMock.Object);

        _hubContextMock.Setup(x => x.Clients).Returns(_hubClientsMock.Object);
        _hubClientsMock.Setup(x => x.All).Returns(_clientProxyMock.Object);
    }

    [Fact]
    public async Task ExecuteReservationStatusUpdateJobAsync_ShouldApproveReservation_WhenRiskEvaluationIsApproved()
    {
        // Arrange
        var reservation = new Reservation("test@example.com", "USA", 100m);
        var reservations = new List<Reservation> { reservation };

        _reservationRepositoryMock
            .Setup(x => x.GetPendingRiskChecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);

        _riskEvaluationServiceMock
            .Setup(x => x.EvaluateAsync(It.IsAny<RiskEvaluationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RiskEvaluationResult { Status = ReservationStatus.Approved });

        var service = new RiskEvaluationBackgroundService(_serviceProviderMock.Object, _loggerMock.Object, _recurringJobManagerMock.Object);

        // Act
        await service.ExecuteReservationStatusUpdateJobAsync();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Approved);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _clientProxyMock.Verify(x => x.SendCoreAsync("ReceiveReservationUpdate", It.Is<object[]>(o => o[0] == reservation), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteReservationStatusUpdateJobAsync_ShouldRejectReservation_WhenRiskEvaluationIsRejected()
    {
        // Arrange
        var reservation = new Reservation("test@example.com", "USA", 100m);
        var reservations = new List<Reservation> { reservation };

        _reservationRepositoryMock
            .Setup(x => x.GetPendingRiskChecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservations);

        _riskEvaluationServiceMock
            .Setup(x => x.EvaluateAsync(It.IsAny<RiskEvaluationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RiskEvaluationResult 
            { 
                Status = ReservationStatus.Rejected,
                RejectionReason = "High risk"
            });

        var service = new RiskEvaluationBackgroundService(_serviceProviderMock.Object, _loggerMock.Object, _recurringJobManagerMock.Object);

        // Act
        await service.ExecuteReservationStatusUpdateJobAsync();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Rejected);
        reservation.RiskReason.Should().Be("High risk");
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _clientProxyMock.Verify(x => x.SendCoreAsync("ReceiveReservationUpdate", It.Is<object[]>(o => o[0] == reservation), It.IsAny<CancellationToken>()), Times.Once);
    }
}
