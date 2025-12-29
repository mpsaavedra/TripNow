using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripNow.Domain.Entities;

namespace TripNow.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.RiskCode)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasData(
            new Country("United States", "US"),
            new Country("Canada", "CA"),
            new Country("Mexico", "MX"),
            new Country("France", "FR")
        );
    }
}
