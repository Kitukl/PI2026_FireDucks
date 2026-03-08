using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(150).IsRequired();

        builder.HasIndex(s => s.Name).IsUnique();

        builder.HasMany(s => s.Lessons)
            .WithOne(l => l.Subject)
            .OnDelete(DeleteBehavior.Restrict);
    }
}