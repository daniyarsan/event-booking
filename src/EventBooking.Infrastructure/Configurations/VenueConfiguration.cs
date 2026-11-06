using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EventBooking.Domain;
using EventBooking.Domain.Venues;

namespace EventBooking.Infrastructure.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");
        builder.HasKey(v => v.Id).HasName("pk_venues");
        builder.Property(v => v.Id).HasConversion(v => v.Value, id => new VenueId(id));

        builder.OwnsOne(v => v.Name, nb =>
        {
            nb.Property(v => v.Prefix)
                .HasMaxLength(LengthConstants.LENGTH50)
                .HasColumnName("prefix");

            nb.Property(v => v.Name)
                .HasMaxLength(LengthConstants.LENGTH500)
                .HasColumnName("name");
        });

        builder.Navigation(v => v.Name).IsRequired(false);
        
        builder.HasMany(v => v.Seats).WithOne().HasForeignKey(v => v.VenueId).IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}