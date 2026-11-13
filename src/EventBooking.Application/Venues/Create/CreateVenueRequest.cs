using EventBooking.Contracts;

namespace EventBooking.Application.Venues.Create;

public record CreateVenueRequest(string prefix, string Name, int SeatsLimit, IEnumerable<CreateSeatRequest> Seats);
