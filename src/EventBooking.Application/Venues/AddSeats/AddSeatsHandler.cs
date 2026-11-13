using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Venues.AddSeats;

public class AddSeatsHandler(IVenueRepository venueRepository, ITransactionManager transactionManager)
{
    public async Task<Result<Guid, Error>> Handle(Guid id, AddSeatsRequest request, CancellationToken cancellationToken)
    {
        if (request.Seats is null)
        {
            return Error.Validation("venue.seats", "Seats are required");
        }

        var seats = new List<Seat>();

        foreach (var item in request.Seats)
        {
            var seatResult = Seat.Create(item.RowNumber, item.SeatNumber);
            if (seatResult.IsFailure)
                return seatResult.Error;

            seats.Add(seatResult.Value);
        }
        
        if (seats.Count == 0)
        {
            return Error.Validation("venue.seats", "Number of seats can not be zero");
        }

        var transactionResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }

        await using var transaction = transactionResult.Value;
        var venueId = new VenueId(id);
        var limit = await venueRepository.GetSeatsLimitForUpdateAsync(venueId, cancellationToken);
        if (limit is null)
        {
            return Error.NotFound("venue.entity.notfound", "Venue not found");
        }

        var venue = await venueRepository.GetVenueWithSeatsForUpdate(venueId, cancellationToken);
        if (venue is null)
        {
            return Error.NotFound("venue.entity.notfound", "Venue not found");
        }

        var addResult = venue.AddSeats(seats);
        if (addResult.IsFailure)
        {
            return addResult.Error;
        }

        venueRepository.AddSeats(seats);
        await venueRepository.SaveChangesAsync(cancellationToken);

        var commitResult = await transaction.CommitAsync(cancellationToken);
        if (commitResult.IsFailure)
        {
            return commitResult.Error;
        }

        return venue.Id.Value;

    }
}
