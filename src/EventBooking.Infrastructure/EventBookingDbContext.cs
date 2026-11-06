using Microsoft.EntityFrameworkCore;

using EventBooking.Domain.Venues;

namespace EventBooking.Infrastructure;

public class EventBookingDbContext(string connectionString) : DbContext
{
    public DbSet<Venue> Venues => Set<Venue>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventBookingDbContext).Assembly);
    }
}

public class VenueRepository(EventBookingDbContext dbContext)
{
    public async Task AddVenue(Venue venue)
    {
        await dbContext.Venues.AddAsync(venue);
    }

    public async Task<List<VenueDto>> GetVenues()
    {
        List<VenueDto> venues = await dbContext.Venues.Select(v => new VenueDto(v.Id.Value, v.Name.Name)).ToListAsync();
        return venues;
    }
}

public record VenueDto(Guid Id, string Name);