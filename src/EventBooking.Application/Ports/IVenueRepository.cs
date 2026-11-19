using CSharpFunctionalExtensions;

using EventBooking.Application.Venues.GetAll;
using EventBooking.Domain.Errors;
using EventBooking.Domain.Venues;

namespace EventBooking.Application.Ports;

public interface IVenueRepository
{
    Task AddVenueAsync(Venue venue, CancellationToken cancellationToken);
    Task<Venue?> GetVenueById(VenueId venueId, CancellationToken cancellationToken);
    Task<Venue?> GetVenueForUpdate(VenueId venueId, CancellationToken cancellationToken);
    Task<Venue?> GetVenueWithSeatsForUpdate(VenueId venueId, CancellationToken cancellationToken);
    Task<List<OneVenueResponse>> GetAllVenues(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<int?> GetSeatsLimitForUpdateAsync(VenueId venueId, CancellationToken cancellationToken);
    Task<List<Seat>> GetSeatsByVenueIdAsync(VenueId venueId, CancellationToken cancellationToken);
    Task LockSeatsForUpdateAsync(IEnumerable<SeatId> seatIds, CancellationToken cancellationToken);
    Task<List<SeatId>> GetReservedSeatIdsByVenueIdAsync(VenueId venueId, CancellationToken cancellationToken);
    void AddSeats(IEnumerable<Seat> seats);
    void RemoveSeats(IEnumerable<Seat> seats);
    Task<int> RemoveVenue(VenueId venueId, CancellationToken cancellationToken);
    Task<bool> HasEventAsync(VenueId venueId, CancellationToken cancellationToken);
}
