using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.UpdatedAt).HasColumnType("timestamp with time zone");
        builder.Property(s => s.IsAutoUpdate).HasDefaultValue(false);
        builder.Property(s => s.CanHeadmanUpdate).HasDefaultValue(true);
    }
}