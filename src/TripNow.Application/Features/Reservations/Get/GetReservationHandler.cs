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

    [WolverineGet("/reservations/{id}")]
    public async Task<Reservation?> Handle(Guid id, IReservationRepository repository, CancellationToken ct)
    {
        return await repository.GetByIdAsync(id, ct);
    }
}
