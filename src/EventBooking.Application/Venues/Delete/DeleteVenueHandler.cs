using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Application.Venues.UpdateSeats;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Venues.Delete;

public class DeleteVenueHandler(IVenueRepository venueRepository)
{
    public async Task<UnitResult<Error>> Handle(Guid id, CancellationToken cancellationToken)
    {
        var venueId = new VenueId(id);
        if (await venueRepository.HasEventAsync(venueId, cancellationToken))
        {
            return Error.Conflict("venue.has_events", "Venue with events cannot be deleted");
        }
        
        var deletedCount = await venueRepository.RemoveVenue(venueId, cancellationToken);
        if (deletedCount == 0)
            return Error.NotFound("venue.not_found", "Venue was not found");
        
        return UnitResult.Success<Error>();
    }
}
