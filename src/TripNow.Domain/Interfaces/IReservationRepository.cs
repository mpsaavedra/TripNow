using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TripNow.Domain.Entities;

namespace TripNow.Domain.Interfaces;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<Reservation?> GetByCustomerEmailAndTripAsync(string customerEmail, string tripCountry, decimal amount, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetRecentAsync(int limit = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetPendingRiskChecksAsync(CancellationToken cancellationToken = default);
}