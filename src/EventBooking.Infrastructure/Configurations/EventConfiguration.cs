using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EventBooking.Domain.Events;
using EventBooking.Domain.Venues;

namespace EventBooking.Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(e => e.Id).HasName("pk_events");
        builder.Property(e => e.Id).HasConversion(e => e.Value, id => new EventId(id)).HasColumnName("id");
        builder.Property(e => e.VenueId).HasColumnName("venue_id");
        
        builder.HasOne<Venue>().WithMany().HasForeignKey(e => e.VenueId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.Property(e => e.Type).HasConversion<string>();
        builder.Property(e => e.EventInfo).HasConversion(i => InfoToString(i), v => StringToInfo(v));

    }
    
    private string InfoToString(IEventInfo eventInfo) => eventInfo switch
    {
        ConcertInfo c => $"Concert:{c.Performer}",
        ConferenceInfo c => $"Conference:{c.Speaker}|{c.Topic}",
        OnlineInfo c => $"Online:{c.Url}",
        _ => throw new NotSupportedException("Unsupported event info type.")
    };

    private IEventInfo StringToInfo(string info)
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

