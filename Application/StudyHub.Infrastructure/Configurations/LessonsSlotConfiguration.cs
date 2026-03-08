using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class LessonsSlotConfiguration : IEntityTypeConfiguration<LessonsSlot>
{
    public void Configure(EntityTypeBuilder<LessonsSlot> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.StartTime).IsRequired();
        builder.Property(l => l.EndTime).IsRequired();

        builder.HasIndex(l => new { l.StartTime, l.EndTime })
            .IsUnique();
    }
}