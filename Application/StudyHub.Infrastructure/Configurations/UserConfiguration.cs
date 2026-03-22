using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Surname).HasMaxLength(100).IsRequired();

        builder.Property(u => u.LastActivity).IsRequired();

        builder.HasMany(u => u.Feedbacks)
            .WithOne(f => f.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Group)
            .WithMany(g => g.Users)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Statistics)
            .WithMany(s => s.Users);

        builder.HasOne(u => u.Reminder);
    }
}