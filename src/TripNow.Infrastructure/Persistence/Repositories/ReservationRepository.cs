using Microsoft.EntityFrameworkCore;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using TripNow.Domain.Enums;

namespace TripNow.Infrastructure.Persistence.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(TripNowDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public Task<Reservation?> GetByCustomerEmailAndTripAsync(string customerEmail, string tripCountry, decimal amount, 
        CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(r => r.CustomerEmail == customerEmail && r.TripCountry == tripCountry, cancellationToken);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IEnumerable<Reservation>> GetPendingRiskChecksAsync(CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _dbSet.Where(r => r.Status == ReservationStatus.PendingRiskCheck);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IEnumerable<Reservation>> GetRecentAsync(int limit = 50, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _dbSet.OrderBy(r => r.CreatedAt).Take(limit).ToList();
    }
}
