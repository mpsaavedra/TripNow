using System;
using System.Collections.Generic;
using System.Text;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using Wolverine.Http;

namespace TripNow.Application.Features.Countries.Get;


public class GetCountriesHandler
{
    [WolverineGet("/countries/")]
    public async Task<IEnumerable<Country>?> Handle(ICountryRepository repository, CancellationToken ct)
    {
        return await repository.GetAllAsync(ct);
    }
}
