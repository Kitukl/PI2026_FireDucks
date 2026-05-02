using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.IsAutoUpdate).HasDefaultValue(false);
        builder.Property(s => s.CanHeadmanUpdate).HasDefaultValue(true);

        builder.HasOne(x => x.Group)
            .WithOne(g => g.Schedule)
            .HasForeignKey<Schedule>(s => s.GroupId)
            .IsRequired();
    }
}