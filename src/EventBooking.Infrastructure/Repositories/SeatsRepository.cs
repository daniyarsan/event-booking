using CSharpFunctionalExtensions;

using EventBooking.Application.Seats;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventBooking.Infrastructure.Repositories;

public class SeatsRepository(EventBookingDbContext dbContext, ILogger<ReservationsRepository> logger) : ISeatsRepository
{
    public async Task<IReadOnlyList<Seat>> GetByIds(IEnumerable<SeatId> seatIds, CancellationToken cancellationToken)
    {
        return await dbContext.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync(cancellationToken);
    }
}