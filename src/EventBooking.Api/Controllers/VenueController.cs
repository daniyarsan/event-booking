using EventBooking.Application.Venues.AddSeats;
using EventBooking.Application.Venues.Create;
using EventBooking.Application.Venues.GetAll;
using EventBooking.Application.Venues.GetById;
using EventBooking.Application.Venues.Update;

using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api.Controllers;

[ApiController]
[Route("api/venues")]
public class VenueController(
    CreateVenueHandler createVenueHandler,
    GetByIdVenueHandler getByIdVenueHandler,
    GetAllVenueHandler getAllVenueHandler,
    UpdateVenueHandler updateVenueHandler,
    AddSeatsHandler addSeatsHandler
    
    ) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVenueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createVenueHandler.Handle(request, cancellationToken);
        return EndpointResponse.From(this, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetByIdVenueRequest(id);
        var result = await getByIdVenueHandler.Handle(request, cancellationToken);

        return EndpointResponse.From(this, result);
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await getAllVenueHandler.Handle(cancellationToken);

        return EndpointResponse.From(this, result);
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateVenueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateVenueHandler.Handle(id, request, cancellationToken);
        return EndpointResponse.From(this, result);
    }
    
    [HttpPatch("{id:guid}/seats")]
    public async Task<IActionResult> AddSeats(
        [FromRoute] Guid id,
        [FromBody] AddSeatsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addSeatsHandler.Handle(id, request, cancellationToken);
        return EndpointResponse.From(this, result);
    }
}
