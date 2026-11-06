using EventBooking.Domain.Venues;

namespace EventBooking.Domain.Reservations;

public record ReservationId(Guid Value);

public class Reservation
{
    public Reservation()
    {
        
    }
    private readonly List<ReservationSeat> _reservationSeats;

    public Reservation(ReservationId id, IEnumerable<Guid> seatIds, Guid eventId, Guid userId)
    {
        Id = id;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        EventId = eventId;
        UserId = userId;

        List<ReservationSeat> reservedSeats =
            seatIds.Select(seatId => new ReservationSeat(new ReservationSeatId(Guid.NewGuid()), this, new SeatId(seatId))).ToList();

        _reservationSeats = reservedSeats;
    }

    public ReservationId Id { get; private set; }

    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }

    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<ReservationSeat> ReservedSeats => _reservationSeats;
}