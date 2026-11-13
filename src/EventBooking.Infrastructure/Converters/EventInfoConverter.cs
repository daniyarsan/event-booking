using EventBooking.Domain.Events;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventBooking.Infrastructure.Converters;


public class EventInfoConverter() : ValueConverter<IEventInfo, string>(i => InfoToString(i), v => StringToInfo(v))
{
    private static string InfoToString(IEventInfo eventInfo) => eventInfo switch
    {
        ConcertInfo c => $"Concert:{c.Performer}",
        ConferenceInfo c => $"Conference:{c.Speaker}|{c.Topic}",
        OnlineInfo c => $"Online:{c.Url}",
        _ => throw new NotSupportedException("Unsupported event info type.")
    };

    private static IEventInfo StringToInfo(string info)
    {
        var split = info.Split(':', 2);
        var type = split[0];
        var data = split[1];

        return type switch
        {
            "Concert" => new ConcertInfo(data),
            "Conference" => new ConferenceInfo(data.Split('|')[0], data.Split('|')[1]),
            "Online" => new OnlineInfo(data),
            _ => throw new NotSupportedException($"Unknown type: {type}")
           
        };
    }
}