using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;

namespace TripNow.Infrastructure.Persistence.Repositories;

public class CountryRepository : GenericRepository<Country>, ICountryRepository
{
    public CountryRepository(TripNowDbContext context) : base(context)
    {
    }

    public async Task<Country?> ExistsAsync(string name, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name, ct);
    }
}
