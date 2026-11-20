using EventBooking.Domain.Venues;

namespace EventBooking.Domain.Events;

public record EventId(Guid Value);

public class Event
{
    public Event()
    {
    }
    public Event(EventId id, VenueId venueId, string name,DateTime eventDate, DateTime startDate, DateTime endDate, IEventInfo eventInfo, EventDetails eventDetails)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        EventInfo = eventInfo;
        StartDate =  startDate;
        EndDate = endDate;
        EventStatus = EventStatus.Planned;
        Details = eventDetails;
    }
    
    public EventId Id { get; private set; }
    public VenueId VenueId { get; private set; }
    public string Name { get; private set; }
    public DateTime EventDate { get; private set; }
    public EventType Type { get; private set; }
    public IEventInfo EventInfo { get; set; }
    public EventDetails Details { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public EventStatus EventStatus { get; private set; }
    public bool IsAvailabelForReservation() => EventStatus != EventStatus.Planned || StartDate >= DateTime.UtcNow;
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

public enum EventStatus
{
    Planned,
    InProgress,
    Finished,
    Cancelled
}