using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Venues.DeleteSeat;

public class DeleteSeatHandler(IVenueRepository venueRepository, ITransactionManager transactionManager)
{
    public async Task<UnitResult<Error>> Handle(Guid venueId, Guid seatId, CancellationToken cancellationToken)
    {
        var beginResult =
            await transactionManager.BeginTransactionAsync(cancellationToken);

        if (beginResult.IsFailure)
            return beginResult.Error;

        await using var transaction = beginResult.Value;

        var venue = await venueRepository.GetVenueWithSeatsForUpdate(
            new VenueId(venueId), cancellationToken);

        if (venue is null)
            return Error.NotFound("venue.not_found", "Venue not found");

        
        
        venue.RemoveSeat(new SeatId(seatId));
        
        await venueRepository.SaveChangesAsync(cancellationToken);
        return await transaction.CommitAsync(cancellationToken);
    }
}