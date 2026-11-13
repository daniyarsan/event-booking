using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Venues.Update;

public class UpdateVenueHandler(IVenueRepository venueRepository)
{
    public async Task<UnitResult<Error>> Handle(Guid id, UpdateVenueRequest request, CancellationToken cancellationToken)
    {
        var venue = await venueRepository.GetVenueForUpdate(new VenueId(id), cancellationToken);
        if (venue is null)
        {
            return Error.NotFound("venue.entity.notfound", "Venue not found");
        }
        
        var updateResult = venue.UpdateDetails(request.Name, request.SeatsLimit);
        if (updateResult.IsFailure)
        {
            return updateResult.Error;
        }
        
        await venueRepository.SaveChangesAsync(cancellationToken);
        
        return UnitResult.Success<Error>();
    }
}
