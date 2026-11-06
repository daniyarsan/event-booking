using CSharpFunctionalExtensions;

using EventBooking.Domain.Errors;

namespace EventBooking.Domain.Venues;

public record SeatId(Guid Value);

public class Seat
{
    private Seat()
    {
    }

    public Seat(SeatId id, int rowNumber, int seatNumber)
    {
        Id = id;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }

    public SeatId Id { get; }
    public VenueId VenueId { get; init; }
    public int RowNumber { get; private set; }

    public int SeatNumber { get; private set; }

    // public Venue Venue { get; private set; } = null!;
    public static Result<Seat, Error> Create(SeatId id, int rowNumber, int seatNumber)
    {
        if (rowNumber <= 0 || seatNumber <= 0)
        {
            return Error.Validation("seat.rowNumber", "Row and seat number should be greater than zero");
        }

        return new Seat(id, rowNumber, seatNumber);
    }
}