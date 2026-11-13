using CSharpFunctionalExtensions;
using EventBooking.Application.Ports;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Venues.Create;

public class CreateVenueHandler(IVenueRepository venueRepository)
{
    public async Task<Result<Guid, Error>> Handle(CreateVenueRequest request, CancellationToken cancellationToken)
    {
        var seats = new List<Seat>();

        foreach (var item in request.Seats)
        {
            var seatResult = Seat.Create(item.RowNumber, item.SeatNumber);
            if (seatResult.IsFailure)
                return seatResult.Error;

            seats.Add(seatResult.Value);
        }

        var venueResult = Venue.Create(
            request.prefix, request.Name, request.SeatsLimit, seats);
        
        
        if (venueResult.IsFailure)
        {
            return venueResult.Error;
        }

        await venueRepository.AddVenueAsync(venueResult.Value, cancellationToken);
        await venueRepository.SaveChangesAsync(cancellationToken);

        return venueResult.Value.Id.Value;
    }
}
