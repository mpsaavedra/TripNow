using System;
using System.Collections.Generic;
using System.Text;

namespace TripNow.Application.Features.Countries.Create;

public record CreateCountry(string Name, string Status);

public record CountryCreatedResponse(Guid Id, string Status);
