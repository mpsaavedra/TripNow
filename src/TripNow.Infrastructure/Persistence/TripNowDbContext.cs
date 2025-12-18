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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is TripNow.Domain.Common.BaseEntity && (
                e.State == EntityState.Added 
                || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((TripNow.Domain.Common.BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((TripNow.Domain.Common.BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
