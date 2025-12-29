using Microsoft.EntityFrameworkCore;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;
using TripNow.Domain.Enums;

namespace TripNow.Infrastructure.Persistence.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(TripNowDbContext context) : base(context)
    {
    }

    public Task<Reservation?> GetByCustomerEmailAndTripAsync(string customerEmail, string tripCountry, decimal amount, 
        CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(r => r.CustomerEmail == customerEmail && r.TripCountry == tripCountry, cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetPendingRiskChecksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(r => r.Status == ReservationStatus.PendingRiskCheck).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetRecentAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await _dbSet.OrderByDescending(r => r.CreatedAt).Take(limit).ToListAsync(cancellationToken);
    }
}
