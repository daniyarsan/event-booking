using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EventBooking.Domain.Reservations;
using EventBooking.Domain.Venues;

namespace EventBooking.Infrastructure.Configurations;

public class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> builder)
    {
        builder.ToTable("reservation_seats");

        builder.HasKey(rs => rs.Id).HasName("pk_reservation_seat");
        
        builder.Property(rs => rs.Id).HasConversion(rs => rs.Value, id => new ReservationSeatId(id)).HasColumnName("id");
        builder.HasOne(rs => rs.Reservation).WithMany(r => r.ReservedSeats).HasForeignKey("reservation_id").IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Seat>().WithMany().HasForeignKey(s => s.SeatId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.Property(rs => rs.SeatId).HasColumnName("seat_id");

    }
}