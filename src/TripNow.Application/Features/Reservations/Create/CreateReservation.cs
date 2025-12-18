using System;
using Wolverine.Http;

namespace TripNow.Application.Features.Reservations.Create;

public record CreateReservation(string CustomerEmail, string TripCountry, decimal Amount);

public record ReservationCreatedResponse(Guid Id, string Status);
