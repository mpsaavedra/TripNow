using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TripNow.Domain.Entities;
using TripNow.Domain.Interfaces;

namespace TripNow.Infrastructure.Persistence;

public class TripNowDbContext : DbContext, IUnitOfWork
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
    
    // IUnitOfWork implementation is inherited from DbContext via SaveChangesAsync, 
    // but strict signature match might be needed if interfaces differ in cancellation token default or something.
    // IUnitOfWork defines: Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    // DbContext has: Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    // So it matches implicitly.
}
