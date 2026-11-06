using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EventBooking.Domain.Events;

namespace EventBooking.Infrastructure.Configurations;

public class EventDetailsConfiguration : IEntityTypeConfiguration<EventDetails>
{
    public void Configure(EntityTypeBuilder<EventDetails> builder)
    {
        builder.ToTable("event_details");

        builder.HasKey(ed => ed.EventId).HasName("pk_event_details");
        builder.Property(ed => ed.EventId).HasConversion(ed => ed.Value, id => new EventId(id)).HasColumnName("event_id");
        builder.HasOne<Event>().WithOne(e => e.Details).HasForeignKey<EventDetails>(ed => ed.EventId).IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}