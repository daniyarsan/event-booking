using CSharpFunctionalExtensions;

using EventBooking.Application.Ports;
using EventBooking.Application.Venues.GetAll;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Reservations;
using EventBooking.Domain.Venues;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Infrastructure;

public class VenuesRepository(EventBookingDbContext dbContext) : IVenueRepository
{
    public async Task AddVenueAsync(Venue venue, CancellationToken cancellationToken)
    {
        await dbContext.Venues.AddAsync(venue, cancellationToken);
    }

    public Task<Venue?> GetVenueById(VenueId venueId, CancellationToken cancellationToken)
    {
        return dbContext.Venues
            .AsNoTracking()
            .Include(venue => venue.Seats)
            .SingleOrDefaultAsync(venue => venue.Id == venueId, cancellationToken);
    }

    public Task<Venue?> GetVenueForUpdate(VenueId venueId, CancellationToken cancellationToken)
    {
        return dbContext.Venues
            .AsTracking()
            .SingleOrDefaultAsync(venue => venue.Id == venueId, cancellationToken);
    }

    public Task<Venue?> GetVenueWithSeatsForUpdate(VenueId venueId, CancellationToken cancellationToken)
    {
        return dbContext.Venues
            .AsTracking()
            .Include(venue => venue.Seats)
            .SingleOrDefaultAsync(venue => venue.Id == venueId, cancellationToken);
    }

    public async Task<Venue?> GetVenueWithSeatsForLock(
        VenueId venueId,
        CancellationToken cancellationToken)
    {
        if (dbContext.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException(
                "An active transaction is required.");
        }
        
        var lockedIds = await dbContext.Database
            .SqlQuery<Guid>($"""
                             SELECT "Id" AS "Value"
                             FROM venues
                             WHERE "Id" = {venueId.Value}
                             FOR UPDATE
                             """)
            .ToListAsync(cancellationToken);
        
        if (lockedIds.Count == 0) 
            return null;

        return await dbContext.Venues
            .AsTracking()
            .Include(venue => venue.Seats)
            .SingleOrDefaultAsync(
                venue => venue.Id == venueId,
                cancellationToken);
    }

    public Task<List<OneVenueResponse>> GetAllVenues(CancellationToken cancellationToken)
    {
        return dbContext.Venues
            .Select(venue => new OneVenueResponse(
                venue.Id.Value,
                venue.Name.Name,
                venue.SeatsLimit,
                venue.Seats.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<int?> GetSeatsLimitForUpdateAsync(VenueId venueId, CancellationToken cancellationToken)
    {
        if (dbContext.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("A transaction is required to update venue seats.");
        }

        var limits = await dbContext.Database.SqlQueryRaw<int>(
                "SELECT \"SeatsLimit\" AS \"Value\" FROM venues WHERE \"Id\" = {0} FOR UPDATE",
                venueId.Value)
            .ToListAsync(cancellationToken);

        return limits.Count == 0 ? null : limits[0];
    }

    public Task<List<Seat>> GetSeatsByVenueIdAsync(VenueId venueId, CancellationToken cancellationToken)
    {
        return dbContext.Set<Seat>()
            .Where(seat => seat.VenueId == venueId)
            .ToListAsync(cancellationToken);
    }

    public async Task LockSeatsForUpdateAsync(IEnumerable<SeatId> seatIds, CancellationToken cancellationToken)
    {
        var ids = seatIds.Select(id => id.Value).ToArray();
        if (ids.Length == 0)
        {
            return;
        }

        await dbContext.Database.SqlQueryRaw<Guid>(
                "SELECT \"Id\" AS \"Value\" FROM seats WHERE \"Id\" = ANY ({0}) FOR UPDATE",
                ids)
            .ToListAsync(cancellationToken);
    }

    public Task<List<SeatId>> GetReservedSeatIdsByVenueIdAsync(VenueId venueId, CancellationToken cancellationToken)
    {
        return dbContext.Set<ReservationSeat>()
            .Join(dbContext.Set<Seat>().Where(seat => seat.VenueId == venueId),
                reservationSeat => reservationSeat.SeatId,
                seat => seat.Id,
                (reservationSeat, seat) => seat.Id)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public void AddSeats(IEnumerable<Seat> seats)
    {
        dbContext.Set<Seat>().AddRange(seats);
    }

    public void RemoveSeats(IEnumerable<Seat> seats)
    {
        dbContext.Set<Seat>().RemoveRange(seats);
    }

    public Task<int> RemoveVenue(VenueId venueId, CancellationToken cancellationToken)
    {
        return dbContext.Venues.Where(v => v.Id == venueId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<List<VenueDto>> GetVenues()
    {
        List<VenueDto> venues = await dbContext.Venues.Select(v => new VenueDto(v.Id.Value, v.Name.Name)).ToListAsync();
        return venues;
    }

    public Task<bool> HasEventAsync(VenueId venueId, CancellationToken cancellationToken)
    {
        return dbContext.Set<Seat>().AnyAsync(seat => seat.VenueId == venueId, cancellationToken);
    }
}

public record VenueDto(Guid Id, string Name);
