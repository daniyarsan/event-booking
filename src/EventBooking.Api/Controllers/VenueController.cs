using EventBooking.Application.Venues.AddSeats;
using EventBooking.Application.Venues.Create;
using EventBooking.Application.Venues.Delete;
using EventBooking.Application.Venues.DeleteSeat;
using EventBooking.Application.Venues.GetAll;
using EventBooking.Application.Venues.GetById;
using EventBooking.Application.Venues.Update;
using EventBooking.Application.Venues.UpdateSeats;

using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api.Controllers;

[ApiController]
[Route("api/venues")]
public class VenueController(
    CreateVenueHandler createVenueHandler,
    GetByIdVenueHandler getByIdVenueHandler,
    GetAllVenueHandler getAllVenueHandler,
    UpdateVenueHandler updateVenueHandler,
    AddSeatsHandler addSeatsHandler,
    UpdateVenueSeatsHandler updateVenueSeatsHandler,
    DeleteVenueHandler deleteVenueHandler,
    DeleteSeatHandler deleteSeatHandler
    
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
    
    [HttpPut("{id:guid}/seats")]
    public async Task<IActionResult> UpdateSeats(
        [FromRoute] Guid id,
        [FromBody] UpdateVenueSeatsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateVenueSeatsHandler.Handle(id, request, cancellationToken);
        return EndpointResponse.From(this, result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await deleteVenueHandler.Handle(id, cancellationToken);
        return EndpointResponse.From(this, result);
    }    
    
    [HttpDelete("{venueId:guid}/seats/{seatId:guid}")]
    public async Task<IActionResult> DeleteSeat(
        [FromRoute] Guid venueId,
        [FromRoute] Guid seatId,
        CancellationToken cancellationToken)
    {
        var result = await deleteSeatHandler.Handle(venueId, seatId, cancellationToken);
        return EndpointResponse.From(this, result);
    }
}
