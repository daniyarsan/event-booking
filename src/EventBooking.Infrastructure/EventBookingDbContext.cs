using EventBooking.Domain.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventBooking.Infrastructure;

public class EventBookingDbContext(string connectionString, ILoggerFactory loggerFactory) : DbContext
{
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Seat> Seat => Set<Seat>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseLoggerFactory(loggerFactory);
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventBookingDbContext).Assembly);
    }
}
