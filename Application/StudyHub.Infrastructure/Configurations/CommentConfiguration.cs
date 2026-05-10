using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(1000);

        builder.HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Feedback)
            .WithMany(f => f.Comments)
            .OnDelete(DeleteBehavior.Cascade);
    }
}