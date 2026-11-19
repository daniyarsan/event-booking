namespace EventBooking.Application.Venues.UpdateSeats;

public record UpdateVenueSeatRequest(Guid? Id, int RowNumber, int SeatNumber);

// Seats is the complete desired layout: null IDs add seats; omitted existing IDs remove them.
public record UpdateVenueSeatsRequest( IReadOnlyList<UpdateVenueSeatRequest> Seats);
