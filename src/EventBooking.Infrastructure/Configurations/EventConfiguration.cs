using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EventBooking.Domain.Events;
using EventBooking.Domain.Venues;
using EventBooking.Infrastructure.Converters;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        builder.Property(e => e.EventInfo).HasConversion(new EventInfoConverter());

    }
    
   
}



