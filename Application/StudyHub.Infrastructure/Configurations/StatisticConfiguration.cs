using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class StatisticConfiguration : IEntityTypeConfiguration<Statistic>
{
    public void Configure(EntityTypeBuilder<Statistic> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CreatedAt).HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(s => s.UserActivityPerMonth).IsRequired();
        builder.Property(s => s.FilesCount).HasDefaultValue(0);

        builder.HasMany(s => s.Tasks)
            .WithMany(t => t.Statistics)
            .UsingEntity(j => j.ToTable("TaskStatistics"));
    }
}