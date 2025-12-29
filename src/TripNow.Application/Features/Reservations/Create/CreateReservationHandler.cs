using System;
using System.Threading;
using System.Threading.Tasks;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using Wolverine.Http;

namespace TripNow.Application.Features.Reservations.Create;

public class CreateReservationHandler
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationHandler(IReservationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }


    [WolverinePost("/reservations")]
    public async Task<ReservationCreatedResponse> Handle(CreateReservation command, CancellationToken ct)
    {
        // Simple idempotency check can be added here if needed, or rely on frontend providing ID.
        // For requirement C "Idempotency": "If create happens twice... result must be consistent."
        // Often this implies providing an Id or having a way to dedupe. User requirement says input has: customerEmail, tripCountry, amount.
        // It doesn't explicitly mention client-generated ID.
        // However, we can check if there's a recent pending reservation for same user/country/amount?
        
        var existing = await _repository.GetByCustomerEmailAndTripAsync(command.CustomerEmail, command.TripCountry, command.Amount, ct);
        if (existing != null && existing.Status == Domain.Enums.ReservationStatus.PendingRiskCheck)
        {
             return new ReservationCreatedResponse(existing.Id, existing.Status.ToString());
        }

        var reservation = new Reservation(command.CustomerEmail, command.TripCountry, command.Amount);
        
        await _repository.AddAsync(reservation, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new ReservationCreatedResponse(reservation.Id, reservation.Status.ToString());
    }
}
