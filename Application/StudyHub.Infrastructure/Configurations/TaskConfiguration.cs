using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StudyHub.Infrastructure.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<StudyHub.Domain.Entities.Task>
{
    public void Configure(EntityTypeBuilder<StudyHub.Domain.Entities.Task> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Deadline).IsRequired();


        builder.HasOne(t => t.Subject)
            .WithMany(s => s.Tasks)
            .OnDelete(DeleteBehavior.Restrict);
    }
}