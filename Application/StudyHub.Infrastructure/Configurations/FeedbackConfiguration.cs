using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

internal class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Type).HasMaxLength(100).IsRequired();
        builder.Property(f => f.Category).HasMaxLength(100).IsRequired();
        builder.Property(f => f.Description).HasMaxLength(1000);
        builder.Property(f => f.CreatorFullname).HasMaxLength(200).IsRequired();
        builder.Property(f => f.UpdatedAt).HasColumnType("timestamp with time zone");
        builder.Property(f => f.ResolvedAt).HasColumnType("timestamp with time zone");
        builder.Property(f => f.CreatedAt).HasColumnType("timestamp with time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(f => f.Status).HasMaxLength(100).IsRequired();
        
    }
}