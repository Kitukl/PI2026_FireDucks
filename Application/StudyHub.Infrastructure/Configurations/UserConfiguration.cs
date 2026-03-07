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
        builder.Property(u => u.PhotoUrl).HasMaxLength(500);

        builder.Property(u => u.LastActivity).HasColumnType("timestamp with time zone").IsRequired();

        builder.HasMany(u => u.Feedbacks)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Group)
            .WithMany(g => g.Users)
            .HasForeignKey(u => u.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Schedule)
            .WithMany(s => s.Users)
            .HasForeignKey(u => u.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Statistics)
            .WithMany(s => s.Users)
            .UsingEntity(j => j.ToTable("UserStatistics"));
    }
}