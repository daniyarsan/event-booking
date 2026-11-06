using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EventBooking.Domain.Reservations;

namespace EventBooking.Infrastructure.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        
        builder.HasKey(r => r.Id).HasName("pk_reservations");
        
        builder.Property(r => r.Id).HasConversion(r => r.Value, id => new ReservationId(id)).HasColumnName("id");
    }
}