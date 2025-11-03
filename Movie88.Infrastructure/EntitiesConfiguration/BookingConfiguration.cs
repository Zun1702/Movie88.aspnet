using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.EntitiesConfiguration;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // Unique index for booking code
        builder.HasIndex(b => b.Bookingcode)
            .IsUnique()
            .HasDatabaseName("IX_Bookings_BookingCode");

        // Note: Default value for Status already set in OnModelCreating
        // Default value for BookingCode will be generated in application code
    }
}
