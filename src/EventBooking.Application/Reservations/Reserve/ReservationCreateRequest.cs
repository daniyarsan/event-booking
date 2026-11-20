namespace EventBooking.Application.Reservations.Create;

public record ReservationCreateRequest(Guid EventId, Guid UserId, IEnumerable<Guid> SeatIds);