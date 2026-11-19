using EventBooking.Application.Ports;
using EventBooking.Application.Venues.AddSeats;
using EventBooking.Application.Venues.Create;
using EventBooking.Application.Venues.Delete;
using EventBooking.Application.Venues.DeleteSeat;
using EventBooking.Application.Venues.GetAll;
using EventBooking.Application.Venues.GetById;
using EventBooking.Application.Venues.Update;
using EventBooking.Application.Venues.UpdateSeats;
using EventBooking.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<EventBookingDbContext>(services =>
    new EventBookingDbContext(builder.Configuration.GetConnectionString("EventBookingDb")!,
        services.GetRequiredService<ILoggerFactory>()));

builder.Services.AddScoped<IVenueRepository, VenuesRepository>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<CreateVenueHandler>();
builder.Services.AddScoped<GetByIdVenueHandler>();
builder.Services.AddScoped<GetAllVenueHandler>();
builder.Services.AddScoped<UpdateVenueHandler>();
builder.Services.AddScoped<UpdateVenueSeatsHandler>();
builder.Services.AddScoped<DeleteVenueHandler>();
builder.Services.AddScoped<DeleteSeatHandler>();
builder.Services.AddScoped<AddSeatsHandler>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "EventBooking v1"));
}

app.UseHttpsRedirection();
app.MapControllers();



app.Run();
