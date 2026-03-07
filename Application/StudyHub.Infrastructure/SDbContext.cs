using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyHub.Domain.Entities;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Infrastructure;

public class SDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
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

    public SDbContext(DbContextOptions<SDbContext> options) :  base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(SDbContext).Assembly);
    }
}