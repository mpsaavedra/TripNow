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
    private readonly ICountryRepository _countryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationHandler(IReservationRepository repository, ICountryRepository countryRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _countryRepository = countryRepository;
        _unitOfWork = unitOfWork;
    }


    [WolverinePost("/reservations")]
    public async Task<ReservationCreatedResponse> Handle(CreateReservation command, CancellationToken ct)
    {
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
