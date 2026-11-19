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

    public Venue(VenueId id, VenueName name, int seatsLimit)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
    }

    public VenueId Id { get; } = null!;
    public VenueName Name { get; private set; }
    public IReadOnlyList<Seat> Seats => _seats;

    public int SeatsLimit { get; private set; }

    public int SeatsCount => _seats.Count;

    public UnitResult<Error> AddSeats(IEnumerable<Seat> seats)
    {
        var newSeats = seats.ToList();
        
        if (newSeats.Count < 1)
        {
            return Error.Validation("venue.seats", "Number of seats can not be zero");
        }
        
        if (_seats.Count + newSeats.Count > SeatsLimit)
        {
            return Error.Conflict("venue.seats.limit", "Number of seats exceeds the venue's limit");
        }

        if (_seats.Concat(newSeats)
            .Select(s => (s.RowNumber, s.SeatNumber))
            .Distinct().Count() != _seats.Count + newSeats.Count)
        {
            return Error.Conflict("venue.seats.duplicate", "Seat positions must be unique");
        }

        foreach (var seat in newSeats)
        {
            seat.AssignToVenue(Id);
        }

        _seats.AddRange(newSeats);
        return UnitResult.Success<Error>();
    }

    public static Result<Venue, Error> Create(string prefix, string name, int seatsLimit, IEnumerable<Seat> seats)
    {
        if (seatsLimit <= 0)
        {
            return Error.Validation("venue.seatsLimit", "limit must be greater than zero");
        }

        var venueNameResult = VenueName.Create(prefix, name);
        if (venueNameResult.IsFailure)
        {
            return venueNameResult.Error;
        }
        

        var venue = new Venue(new VenueId(Guid.NewGuid()), venueNameResult.Value, seatsLimit);
        var addResult = venue.AddSeats(seats); 
        if (addResult.IsFailure) 
            return addResult.Error;
        

        return venue;
    }

    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
        {
            return Error.Conflict("venue.seats.limit", "");
        }

        return AddSeats([seat]);
    }
    
    public UnitResult<Error> RemoveSeat(SeatId seatId)
    {
       var seat = _seats.SingleOrDefault(seat => seat.Id == seatId);
       if (seat is null)
       {
           return Error.NotFound("venue.seats.notFound", "Seat not found");
       }

       if (_seats.Count <= 1)
       {
           return Error.NotFound("venue.seats_min_limit", "Seats count is less than 1");
       }
       
       _seats.Remove(seat);
       
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

    public UnitResult<Error> UpdateDetails(string name, int seatsLimit)
    {
        var nameResult = VenueName.Create(Name.Prefix, name);
        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }

        var limitResult = IncreaseSeatsLimit(seatsLimit);
        if (limitResult.IsFailure)
        {
            return limitResult.Error;
        }

        Name = nameResult.Value;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddName(string name)
    {
        var nameResult = VenueName.Create(Name.Prefix, name);
        if (nameResult.IsFailure)
        {
            return nameResult.Error;
        }
        
        Name = nameResult.Value;
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> IncreaseSeatsLimit(int seatsLimit)
    {
        if (seatsLimit <= 0)
        {
            return Error.Validation("venue.seatsLimit", "Seats limit must be greater than zero");
        }

        if (seatsLimit < SeatsLimit)
        {
            return Error.Validation("venue.validation.error", "Seats limit cannot be lower than existing value");
        }

        SeatsLimit = seatsLimit;
        
        return UnitResult.Success<Error>();
    }
}
