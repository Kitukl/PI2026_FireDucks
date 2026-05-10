using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

internal class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Description).HasMaxLength(1000);
        builder.Property(f => f.CreatorFullname).HasMaxLength(200).IsRequired();

        builder.HasMany(f => f.Comments)
            .WithOne(c => c.Feedback)
            .OnDelete(DeleteBehavior.Restrict);
        
    }
}