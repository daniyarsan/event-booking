using CSharpFunctionalExtensions;

using EventBooking.Application.Events;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using EventId = EventBooking.Domain.Events.EventId;

namespace EventBooking.Infrastructure.Repositories;

public class EventsRepository(EventBookingDbContext dbContext, ILogger<ReservationsRepository> logger) : IEventsRepository
{
    public async Task<Result<Event, Error>> GetById(EventId eventId, CancellationToken cancellationToken)
    {
        var @event = await dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (@event is null)
        {
            return Error.Failure("event.not.found", "Event not found");
        }

        return @event;
    }
}