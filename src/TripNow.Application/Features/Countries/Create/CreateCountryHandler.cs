using System;
using System.Collections.Generic;
using System.Text;
using TripNow.Application.Features.Reservations.Create;
using TripNow.Domain.Interfaces;
using Wolverine.Http;

namespace TripNow.Application.Features.Countries.Create;

public class CreateCountryHandler
{
    private readonly ICountryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCountryHandler(ICountryRepository countryRepository, IUnitOfWork unitOfWork)
    {
        _repository = countryRepository;
        _unitOfWork = unitOfWork;
    }

    [WolverinePost("/countries")]
    public async Task<CountryCreatedResponse> Handle(CreateCountry command, CancellationToken ct)
    {
        var existing = await _repository.ExistsAsync(command.Name, ct);
        if (existing != null)
        {
            return new CountryCreatedResponse(existing.Id, "exists");
        }

        var country = new Domain.Entities.Country(command.Name, "created");

        await _repository.AddAsync(country, ct);
        await _unitOfWork.SaveChangesAsync(ct);


        return new CountryCreatedResponse(country.Id, "created");
    }
}
