using System.Linq.Expressions;
using TripNow.Domain.Entities;

namespace TripNow.Domain.Interfaces;

public interface ICountryRepository : IGenericRepository<Country>
{
    Task<Country?> ExistsAsync(string name, CancellationToken ct);
}
