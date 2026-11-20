using CSharpFunctionalExtensions;

using EventBooking.Domain.Errors;
using EventBooking.Domain.Events;

namespace EventBooking.Application.Events;

public interface IEventsRepository
{
    Task<Result<Event, Error>> GetById(EventId eventId, CancellationToken cancellationToken);
}