using EventBooking.Application.Reservations.Create;

using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api.Controllers;

[ApiController]
[Route("api/venues")]
public class ReservationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Reserve(
        [FromBody] ReservationCreateRequest request, 
        [FromServices] ReservationCreateHandler  handler,  
        CancellationToken  cancellationToken)
    {
        var result = await handler.Handle(request, cancellationToken);
        return EndpointResponse.From(this, result);
    }
}