using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Application.Reservations;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Reservations;
using EventBooking.Domain.Venues;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventBooking.Infrastructure.Repositories;

public class ReservationsRepository(EventBookingDbContext dbContext, ILogger<ReservationsRepository> logger) : IReservationsRepository
{
    private readonly EventBookingDbContext _dbContext = dbContext;
    private readonly ILogger<ReservationsRepository> _logger = logger;

    public async Task<Result<Guid, Error>> Add(Reservation reservation, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return reservation.Id.Value;
        }
        catch (Exception e)
        {
           _logger.LogError(e, "Fail to insert reservation");
           return Error.Failure("venue.insert", "Fail to insert reservation");
        }
    }

    public async Task<bool> AnySeatsAlreadyReserved(Guid eventId, IEnumerable<SeatId> seatIds,
        CancellationToken cancellationToken)
    {
        var hasReservedSeats = await _dbContext.Reservations
            .Where(r => r.EventId == eventId)
            .Where(r => r.ReservedSeats.Any(rs => seatIds.Contains(rs.SeatId)))
            .AnyAsync(cancellationToken: cancellationToken);

        return hasReservedSeats;
    }
}