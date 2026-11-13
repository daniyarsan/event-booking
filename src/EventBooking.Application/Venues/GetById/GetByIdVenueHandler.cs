using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Venues.GetById;

public class GetByIdVenueHandler(IVenueRepository venueRepository)
{
    public async Task<Result<GetByIdVenueResponse, Error>> Handle(
        GetByIdVenueRequest request,
        CancellationToken cancellationToken)
    {
        var venue = await venueRepository.GetVenueById(new VenueId(request.VenueId), cancellationToken);

        if (venue is null)
        {
            return Error.NotFound("venue.not_found", "Venue was not found");
        }

        return new GetByIdVenueResponse(
            venue.Id.Value,
            venue.Name.Prefix,
            venue.Name.Name,
            venue.SeatsLimit,
            venue.Seats
                .Select(seat => new SeatResponse(seat.Id.Value, seat.RowNumber, seat.SeatNumber))
                .ToList());
    }
}
