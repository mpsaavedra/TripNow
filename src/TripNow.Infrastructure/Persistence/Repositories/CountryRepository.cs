using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;

namespace TripNow.Infrastructure.Persistence.Repositories;

public class CountryRepository : GenericRepository<Country>, ICountryRepository
{
    public CountryRepository(TripNowDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
