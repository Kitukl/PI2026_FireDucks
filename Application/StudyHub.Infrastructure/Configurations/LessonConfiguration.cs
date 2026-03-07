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
            .HasForeignKey(l => l.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Schedules)
            .WithMany(sc => sc.Lessons)
            .UsingEntity(j => j.ToTable("LessonSchedules"));

        builder.HasMany(l => l.LessonsSlots)
            .WithMany(ls => ls.Lessons)
            .UsingEntity(j => j.ToTable("LessonToSlot"));

        builder.HasMany(l => l.Lecturers)
            .WithMany(lecturer => lecturer.Lessons)
            .UsingEntity(j => j.ToTable("LecturerLessons"));
    }
}