namespace EventBooking.Domain.Events;


public class EventDetails
{
    public EventDetails()
    {
        
    }
    public EventDetails(int capacity, string description)
    {
        Capacity = capacity;
        Description = description;
    }

    public EventId EventId {get; init;}
    public int Capacity { get; private set; }
    public string Description { get; private set; }
}