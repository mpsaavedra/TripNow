using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TripNow.Domain.Entities;
using TripNow.Infrastructure.Persistence;
using TripNow.Infrastructure.Persistence.Repositories;
using Xunit;

namespace TripNow.UnitTests.Persistence;

public class ReservationRepositoryTests
{
    private readonly DbContextOptions<TripNowDbContext> _options;

    public ReservationRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TripNowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddReservation()
    {
        // Arrange
        using var context = new TripNowDbContext(_options);
        var repository = new ReservationRepository(context);
        var reservation = new Reservation("test@test.com", "US", 100);

        // Act
        await repository.AddAsync(reservation);
        await context.SaveChangesAsync();

        // Assert
        using var assertContext = new TripNowDbContext(_options);
        var saved = await assertContext.Reservations.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.CustomerEmail.Should().Be("test@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnReservation()
    {
        // Arrange
        var reservation = new Reservation("test2@test.com", "US", 200);
        using (var context = new TripNowDbContext(_options))
        {
            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TripNowDbContext(_options))
        {
            var repository = new ReservationRepository(context);
            var result = await repository.GetByIdAsync(reservation.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(reservation.Id);
        }
    }

    [Fact]
    public async Task Remove_ShouldDeleteReservation()
    {
        // Arrange
        var reservation = new Reservation("delete@test.com", "US", 300);
        using (var context = new TripNowDbContext(_options))
        {
            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TripNowDbContext(_options))
        {
            var repository = new ReservationRepository(context);
            // Re-fetch to attach or just attach
            var toDelete = await repository.GetByIdAsync(reservation.Id);
            repository.Remove(toDelete!);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TripNowDbContext(_options))
        {
            var repository = new ReservationRepository(context);
            var result = await repository.GetByIdAsync(reservation.Id);
            result.Should().BeNull();
        }
    }
}
