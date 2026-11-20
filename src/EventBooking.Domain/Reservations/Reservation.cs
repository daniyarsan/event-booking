using CSharpFunctionalExtensions;

using EventBooking.Domain.Errors;
using EventBooking.Domain.Events;
using EventBooking.Domain.Venues;

namespace EventBooking.Domain.Reservations;

public record ReservationId(Guid Value);

public class Reservation
{
    public Reservation()
    {
        
    }
    private readonly List<ReservationSeat> _reservationSeats;

    public Reservation(ReservationId id, Guid eventId, Guid userId, IEnumerable<Guid> seatIds)
    {
        Id = id;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        EventId = eventId;
        UserId = userId;

        List<ReservationSeat> reservedSeats =
            seatIds.Select(seatId => new ReservationSeat(new ReservationSeatId(Guid.NewGuid()), this, new SeatId(seatId), new EventId(eventId))).ToList();

        _reservationSeats = reservedSeats;
    }


    public ReservationId Id { get; private set; }

    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<ReservationSeat> ReservedSeats => _reservationSeats;
    
    public static Result<Reservation, Error> Create(Guid eventId, Guid userId, IEnumerable<Guid> seatsId) 
    {
        if (eventId == Guid.Empty)
        {
            return Error.Validation("reservation.eventId", "Event ID cannot be empty");
        }

        if (userId == Guid.Empty)
        {
            return Error.Validation("reservation.userId", "User ID cannot be empty");
        }

        var seatsIdsList = seatsId.ToList() ?? [];

        if (seatsIdsList.Count == 0)
        {
            return Error.Validation("reservation.seats", "At Least one seat must be selected");
        }

        if (seatsIdsList.Any(seatId => seatId == Guid.Empty))
        {
            return Error.Validation("reservation.seats", "Seat IDs cannot be empty");
        }

        return new Reservation(new ReservationId(Guid.NewGuid()), eventId, userId, seatsIdsList);


    }

}