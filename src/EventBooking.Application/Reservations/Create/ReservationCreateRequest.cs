namespace EventBooking.Application.Reservations.Create;

public record ReservationCreateRequest(Guid Id, Guid UserId, IEnumerable<Guid> SeatIds);