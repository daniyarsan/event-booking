using EventBooking.Contracts;

namespace EventBooking.Application.Venues.AddSeats;

public record AddSeatsRequest( IEnumerable<CreateSeatRequest> Seats);
