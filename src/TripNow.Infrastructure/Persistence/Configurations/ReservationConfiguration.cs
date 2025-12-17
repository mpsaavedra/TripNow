using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripNow.Domain.Entities;

namespace TripNow.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.CustomerEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.TripCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.Status)
            .HasConversion<string>();

        builder.Property(r => r.RiskReason)
            .HasMaxLength(500);
            
        builder.Ignore(r => r.DomainEvents);
    }
}
