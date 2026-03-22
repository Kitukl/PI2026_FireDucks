using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(g => g.Name).IsUnique();
        
        builder.HasOne(g => g.Schedule)
            .WithOne(s => s.Group)
            .OnDelete(DeleteBehavior.Restrict);
    }
}