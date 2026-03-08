using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(l => l.Subject)
            .WithMany(su => su.Lessons)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Schedules)
            .WithMany(sc => sc.Lessons);
        builder.HasOne(l => l.LessonsSlot)
            .WithMany(ls => ls.Lessons);

        builder.HasMany(l => l.Lecturers)
            .WithMany(lecturer => lecturer.Lessons);
        
    }
}