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
    public VenueId VenueId { get; private set; } = null!;
    public int RowNumber { get; private set; }

    public int SeatNumber { get; private set; }

    public static Result<Seat, Error> Create(int rowNumber, int seatNumber)
    {
        if (rowNumber <= 0 || seatNumber <= 0)
        {
            return Error.Validation("seat.rowNumber", "Row and seat number should be greater than zero");
        }

        return new Seat(new SeatId(Guid.NewGuid()), rowNumber, seatNumber);
    }
    

    public static Result<Seat, Error> Create(VenueId venueId, int rowNumber, int seatNumber)
    {
        var result = Create(rowNumber, seatNumber);
        if (result.IsFailure)
        {
            return result.Error;
        }

        result.Value.AssignToVenue(venueId);
        return result.Value;
    }

    public UnitResult<Error> ChangePosition(int rowNumber, int seatNumber)
    {
        if (rowNumber <= 0 || seatNumber <= 0)
        {
            return Error.Validation("seat.position", "Row and seat number should be greater than zero");
        }

        RowNumber = rowNumber;
        SeatNumber = seatNumber;
        return UnitResult.Success<Error>();
    }

    internal void AssignToVenue(VenueId venueId) => VenueId = venueId;
}
