using CSharpFunctionalExtensions;

using EventBooking.Domain.Errors;
using EventBooking.Domain.Reservations;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Reservations;

public interface IReservationsRepository
{
    Task<Result<Guid, Error>> Add(Reservation reservation, CancellationToken cancellationToken);

    Task<bool> AnySeatsAlreadyReserved(Guid eventId, IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken);
}