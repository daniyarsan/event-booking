using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EventBooking.Domain;

namespace EventBooking.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id).HasName("pk_users");
        builder.Property(u => u.Id).HasConversion(u => u.Value, id => new UserId(id));
        
        builder.OwnsOne(u => u.Details, db =>
        {
            db.ToJson("details");
            db.OwnsMany(d => d.SocialNetworks, sb =>
            {
                sb.Property(u => u.Link).IsRequired().HasMaxLength(LengthConstants.LENGTH500).HasJsonPropertyName("link");
                sb.Property(u => u.Name).IsRequired().HasMaxLength(LengthConstants.LENGTH500).HasJsonPropertyName("name");
            });
            
            db.Property(u => u.Description).IsRequired().HasMaxLength(LengthConstants.LENGTH500).HasJsonPropertyName("description");
        });
        

    }
}