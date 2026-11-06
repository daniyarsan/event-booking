using CSharpFunctionalExtensions;

using EventBooking.Domain.Errors;

namespace EventBooking.Domain.Venues;

public record VenueId(Guid Value);

public record VenueName(string Name, string Prefix)
{
    public override string ToString()
    {
        return $"{Prefix}-{Name}";
    }

    public static Result<VenueName, Error> Create(string prefix, string name)
    {
        if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("venue.name", "name cannot be empty");
        }

        if (prefix.Length > LengthConstants.LENGTH50 || name.Length > LengthConstants.LENGTH500)
        {
            return Error.Validation("venue.name", "name is too long");
        }

        return new VenueName(name, prefix);
    }
}

public class Venue
{
    private readonly List<Seat> _seats = [];

    private Venue()
    {
    }

    public Venue(VenueId id, VenueName name, int seatsLimit, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
        _seats = seats.ToList();
    }

    public VenueId Id { get; } = null!;
    public VenueName Name { get; private set; }
    public IReadOnlyList<Seat> Seats => _seats;

    public int SeatsLimit { get; private set; }

    public int SeatsCount => _seats.Count;

    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
        {
            return Error.Conflict("venue.seats.limit", "");
        }

        _seats.Add(seat);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> ExpandSeatsLimit(int newSeatsLimit)
    {
        if (newSeatsLimit < SeatsLimit)
        {
            return Error.Conflict("Wrong seats limit", "");
        }

        SeatsLimit = newSeatsLimit;

        return Result.Success<Error>();
    }
}