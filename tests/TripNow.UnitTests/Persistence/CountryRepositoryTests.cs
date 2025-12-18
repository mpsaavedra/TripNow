using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TripNow.Infrastructure.Persistence;

namespace TripNow.UnitTests.Persistence;

public class CountryRepositoryTests
{

    private readonly DbContextOptions<TripNowDbContext> _options;

    public CountryRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TripNowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
}
