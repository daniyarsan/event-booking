using CSharpFunctionalExtensions;

using EventBooking.Application.Events;
using EventBooking.Application.Reservations.Create;
using EventBooking.Application.Seats;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Events;
using EventBooking.Domain.Reservations;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Reservations.Reserve;

public class ReservationCreateHandler(IReservationsRepository reservationsRepository, IEventsRepository eventsRepository, ISeatsRepository seatsRepository)
{
    public async Task<Result<Guid, Error>> Handle(ReservationCreateRequest request, CancellationToken cancellationToken)
    { 
        // 1. Validation of params Fluent validation
        // 2. Business logic check. Check reservation: date, status
        var eventId = new EventId(request.EventId);
        var (_, isFailure, @event, error) = await eventsRepository.GetById(eventId, cancellationToken);
        if (isFailure)
        {
            return error;
        }
        
        if (@event.IsAvailabelForReservation() == false)
        {
            return Error.Failure("reservation.unavailable", "Reservation is unavailable");
        }
        
        // 3. Seats belong to venue and event
        var seatIds = request.SeatIds.Select(id => new SeatId(id)).ToList();
        var seats = await seatsRepository.GetByIds(seatIds, cancellationToken);

        if (seats.Any(seat => seat.VenueId != @event.VenueId) || seats.Count == 0)
        {
            return Error.Conflict("seat.conflict", "Seat does not belong to this venue");
        }
        
        // 4. Check that seats are not reserved
        // var areSeatsReserved = await reservationsRepository.AnySeatsAlreadyReserved(@event.Id.Value, seatIds, cancellationToken);
        // if (areSeatsReserved)
        // {
        //     return Error.Conflict("seats.conflict",  "Seat already reserved");
        // }
        
        var reservationResult = Reservation.Create(request.EventId, request.UserId, request.SeatIds);
        if (reservationResult.IsFailure)
        {
            return reservationResult.Error;
        }

        await reservationsRepository.Add(reservationResult.Value, cancellationToken);

        return reservationResult.Value.Id.Value;
    }
}