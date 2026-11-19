using CSharpFunctionalExtensions;

using EventBooking.Domain.Errors;

namespace EventBooking.Application.Reservations.Create;

public class ReservationCreateHandler
{
    public async Task<Result<Guid, Error>> Handle(ReservationCreateRequest request, CancellationToken cancellationToken)
    {
        var
    }
}