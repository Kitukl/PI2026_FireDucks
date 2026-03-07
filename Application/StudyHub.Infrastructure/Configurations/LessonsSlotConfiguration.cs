using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class LessonsSlotConfiguration : IEntityTypeConfiguration<LessonsSlot>
{
    public void Configure(EntityTypeBuilder<LessonsSlot> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.StartTime).HasColumnType("time").IsRequired();
        builder.Property(l => l.EndTime).HasColumnType("time").IsRequired();

        builder.HasIndex(l => new { l.StartTime, l.EndTime })
            .IsUnique();
    }
}