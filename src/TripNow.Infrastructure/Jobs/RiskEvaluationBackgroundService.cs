using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TripNow.Domain.Entities;
using TripNow.Domain.Enums;
using TripNow.Domain.Interfaces;
using TripNow.Domain.Services;
using TripNow.Infrastructure.Hubs;

namespace TripNow.Infrastructure.Jobs;

public interface IRiskEvaluationBackgroundService
{
    void ScheduleRecurringJobs();
    Task ExecuteReservationStatusUpdateJobAsync();
}

public class RiskEvaluationBackgroundService : IRiskEvaluationBackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RiskEvaluationBackgroundService> _logger;
    private readonly IRecurringJobManager _recurringJobManager;

    public RiskEvaluationBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RiskEvaluationBackgroundService> logger,
        IRecurringJobManager recurringJobManager)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _recurringJobManager = recurringJobManager;
    }

    public void ScheduleRecurringJobs()
    {
        _recurringJobManager.AddOrUpdate(
               "reservation-status-checker",
               () => ExecuteReservationStatusUpdateJobAsync(),
               "* * * * *" // Cron expression for every minute
           );

        _logger.LogInformation("Scheduled recurring job: reservation-status-checker (every minute)");
    }

    public async Task ExecuteReservationStatusUpdateJobAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
        var riskEvaluationService = scope.ServiceProvider.GetRequiredService<IRiskEvaluationService>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ReservationHub>>();

        var pendingReservations = await reservationRepository.GetPendingRiskChecksAsync();

        foreach (var reservation in pendingReservations)
        {
            try
            {
                _logger.LogInformation("Evaluating risk for reservation {ReservationId}", reservation.Id);

                var request = new RiskEvaluationRequest
                {
                    CustomerEmail = reservation.CustomerEmail,
                    TripCountry = reservation.TripCountry,
                    Amount = reservation.Amount
                };

                var result = await riskEvaluationService.EvaluateAsync(request);

                if (result.Status == ReservationStatus.Approved)
                {
                    reservation.Approve();
                    await unitOfWork.SaveChangesAsync();
                    await hubContext.Clients.All.SendAsync("ReceiveReservationUpdate", reservation);

                    _logger.LogInformation("Reservation {ReservationId} approved.", reservation.Id);
                }
                else if (result.Status == ReservationStatus.Rejected)
                {
                    reservation.Reject(result.RejectionReason ?? "Rejected by risk evaluation service");
                    await unitOfWork.SaveChangesAsync();
                    await hubContext.Clients.All.SendAsync("ReceiveReservationUpdate", reservation);

                    _logger.LogInformation("Reservation {ReservationId} rejected: {Reason}", reservation.Id, result.RejectionReason);
                }

                //await unitOfWork.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process risk evaluation for reservation {ReservationId}", reservation.Id);
            }
        }
    }
}
