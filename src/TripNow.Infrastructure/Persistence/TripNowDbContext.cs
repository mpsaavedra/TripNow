using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;

namespace TripNow.Infrastructure.Persistence;

public class TripNowDbContext : DbContext
{
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Country> Countries { get; set; }

    public TripNowDbContext(DbContextOptions<TripNowDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
