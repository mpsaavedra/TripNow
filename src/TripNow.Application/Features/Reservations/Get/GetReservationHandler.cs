using System;
using System.Threading;
using System.Threading.Tasks;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using Wolverine.Http;

namespace TripNow.Application.Features.Reservations.Get;

public record GetReservation(Guid Id);

public class GetReservationHandler
{
    private readonly IReservationRepository _repository;

    public GetReservationHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    [WolverineGet("/reservations/{Id}")]
    public async Task<Reservation?> Handle(GetReservation query, CancellationToken ct)
    {
        return await _repository.GetByIdAsync(query.Id, ct);
    }
}
