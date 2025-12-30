using System;
using System.Collections.Generic;
using System.Text;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using Wolverine.Http;

namespace TripNow.Application.Features.Reservations.Get;

public class GetReservationsHandler
{
    [WolverineGet("/reservations/")]
    public async Task<IEnumerable<Reservation>?> Handle(IReservationRepository repository, CancellationToken ct)
    {
        return await repository.GetAllAsync(ct);
    }
}
