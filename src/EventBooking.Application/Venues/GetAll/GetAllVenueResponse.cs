namespace EventBooking.Application.Venues.GetAll;

public record OneVenueResponse(Guid VenueId, string Name, int SeatsLimit, int SeatsCount);
public record GetAllVenueResponse(IEnumerable<OneVenueResponse> Venues, int VenuesCount);