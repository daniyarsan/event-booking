namespace EventBooking.Application.Venues.GetById;

public record SeatResponse(Guid Id, int RowNumber, int SeatNumber);

public record GetByIdVenueResponse(
    Guid Id,
    string Prefix,
    string Name,
    int SeatsLimit,
    IReadOnlyList<SeatResponse> Seats);
