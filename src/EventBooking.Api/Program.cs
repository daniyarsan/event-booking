using EventBooking.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddScoped<EventBookingDbContext>(_ =>
    new EventBookingDbContext(builder.Configuration.GetConnectionString("EventBookingDb")!));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/weatherforecast", () =>
    {
        return "asdf";
    })
    .WithName("GetWeatherForecast");

app.Run();