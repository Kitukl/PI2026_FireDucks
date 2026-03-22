using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Infrastructure;

public class SDbContext(DbContextOptions<SDbContext> options) 
    : IdentityDbContext<User, IdentityRole<Guid>, Guid> (options)
{
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<LessonsSlot> LessonsSlots { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Statistic> Statistics { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = nameof(Role.Student),
                NormalizedName = "STUDENT"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = nameof(Role.Admin),
                NormalizedName = "ADMIN"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = nameof(Role.Leader),
                NormalizedName = "LEADER"
            }
        );
        
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(SDbContext).Assembly);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}