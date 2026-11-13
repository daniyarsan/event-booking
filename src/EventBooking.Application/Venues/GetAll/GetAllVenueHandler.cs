using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Domain.Errors;

namespace EventBooking.Application.Venues.GetAll;

public class GetAllVenueHandler(IVenueRepository venueRepository)
{
    public async Task<Result<GetAllVenueResponse, Error>> Handle(
        CancellationToken cancellationToken)
    {
        var venues = await venueRepository.GetAllVenues(cancellationToken);
        return new GetAllVenueResponse(venues, venues.Count);
    }
}
