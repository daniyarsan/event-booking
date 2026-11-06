using EventBooking.Domain.Venues;

namespace EventBooking.Domain.Events;

public record EventId(Guid Value);

public class Event
{
    public Event()
    {
    }
    public Event(EventId id, VenueId venueId, EventDetails eventDetails, string name, DateTime eventDate)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
    }



    public EventId Id { get; private set; }
    public VenueId VenueId { get; private set; }
    public string Name { get; private set; }
    public DateTime EventDate { get; private set; }

    public EventType Type { get; private set; }
    public IEventInfo EventInfo { get; set; }
    public EventDetails Details { get; private set; }
}

public enum EventType
{
    Concert,
    Conference,
    Online
}

public interface IEventInfo
{
}

public record ConcertInfo(string Performer) : IEventInfo;
public record ConferenceInfo(string Speaker, string Topic) : IEventInfo;
public record OnlineInfo(string Url) : IEventInfo;