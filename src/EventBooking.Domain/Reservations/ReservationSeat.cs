using EventBooking.Domain.Venues;

namespace EventBooking.Domain.Reservations;

public record ReservationSeatId(Guid Value);

public class ReservationSeat
{
    public ReservationSeat()
    {
        
    }
    public ReservationSeat(ReservationSeatId id, Reservation reservation, SeatId seatId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservedAt = DateTime.UtcNow;
    }

    public ReservationSeatId Id { get; }
    public Reservation Reservation { get; private set; }
    public SeatId SeatId { get; private set; }
    public DateTime ReservedAt { get; }
}