using CSharpFunctionalExtensions;
using EventBooking.Application.Ports;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Venues.Update;

public class UpdateVenueSeatsHandler(IVenueRepository venueRepository, ITransactionManager transactionManager)
{
    public async Task<UnitResult<Error>> Handle(UpdateVenueSeatsRequest request, CancellationToken cancellationToken)
    {
        if (request.Seats is null || request.Seats.Count == 0)
        {
            return Error.Validation("venue.seats", "A venue must have at least one seat");
        }

        if (request.Seats.Any(seat => seat.RowNumber <= 0 || seat.SeatNumber <= 0))
        {
            return Error.Validation("seat.position", "Row and seat number should be greater than zero");
        }

        if (request.Seats.Select(seat => (seat.RowNumber, seat.SeatNumber)).Distinct().Count() != request.Seats.Count)
        {
            return Error.Conflict("venue.seats.duplicate", "Seat positions must be unique");
        }

        var existingIds = request.Seats.Where(seat => seat.Id.HasValue).Select(seat => seat.Id!.Value).ToList();
        if (existingIds.Distinct().Count() != existingIds.Count)
        {
            return Error.Validation("venue.seats.ids", "Seat IDs must be unique");
        }

        var transactionResult = await transactionManager.BeginTransactionAsync(cancellationToken);
        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }

        await using var transaction = transactionResult.Value;
        var venueId = new VenueId(request.VenueId);
        var seatsLimit = await venueRepository.GetSeatsLimitForUpdateAsync(venueId, cancellationToken);
        if (seatsLimit is null)
        {
            return Error.NotFound("venue.not_found", "Venue was not found");
        }

        if (request.Seats.Count > seatsLimit.Value)
        {
            return Error.Conflict("venue.seats.limit", "Number of seats exceeds the venue's limit");
        }

        var currentSeats = await venueRepository.GetSeatsByVenueIdAsync(venueId, cancellationToken);
        var currentById = currentSeats.ToDictionary(seat => seat.Id.Value);
        if (existingIds.Any(id => !currentById.ContainsKey(id)))
        {
            return Error.Validation("venue.seats.ids", "A seat does not belong to this venue");
        }

        var retainedIds = existingIds.ToHashSet();
        var removedSeats = currentSeats.Where(seat => !retainedIds.Contains(seat.Id.Value)).ToList();
        var movedSeats = request.Seats
            .Where(seat => seat.Id.HasValue &&
                           (currentById[seat.Id.Value].RowNumber != seat.RowNumber ||
                            currentById[seat.Id.Value].SeatNumber != seat.SeatNumber))
            .ToList();

        if (removedSeats.Count > 0 || movedSeats.Count > 0)
        {
            await venueRepository.LockSeatsForUpdateAsync(
                removedSeats.Select(seat => seat.Id)
                    .Concat(movedSeats.Select(seat => currentById[seat.Id!.Value].Id)),
                cancellationToken);

            var reservedIds = (await venueRepository.GetReservedSeatIdsByVenueIdAsync(venueId, cancellationToken))
                .Select(id => id.Value)
                .ToHashSet();
            if (removedSeats.Any(seat => reservedIds.Contains(seat.Id.Value)) ||
                movedSeats.Any(seat => reservedIds.Contains(seat.Id!.Value)))
            {
                return Error.Conflict("venue.seats.reserved", "Reserved seats cannot be moved or removed");
            }
        }

        foreach (var requestedSeat in movedSeats)
        {
            var changeResult = currentById[requestedSeat.Id!.Value]
                .ChangePosition(requestedSeat.RowNumber, requestedSeat.SeatNumber);
            if (changeResult.IsFailure)
            {
                return changeResult.Error;
            }
        }

        var newSeats = new List<Seat>();
        foreach (var requestedSeat in request.Seats.Where(seat => !seat.Id.HasValue))
        {
            var seatResult = Seat.Create(venueId, requestedSeat.RowNumber, requestedSeat.SeatNumber);
            if (seatResult.IsFailure)
            {
                return seatResult.Error;
            }

            newSeats.Add(seatResult.Value);
        }

        venueRepository.RemoveSeats(removedSeats);
        venueRepository.AddSeats(newSeats);
        await venueRepository.SaveChangesAsync(cancellationToken);
        return await transaction.CommitAsync(cancellationToken);
    }
}
