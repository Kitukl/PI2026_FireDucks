using Microsoft.AspNetCore.Identity;

namespace StudyHub.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime LastActivity { get; set; }
    public Guid MicrosoftId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public bool IsNotified { get; set; }
    public TimeSpan ReminderOffset { get; set; }
    public string PhotoUrl { get; set; }

    public Guid GroupId { get; set; }
    public Group Group { get; set; }

    public Guid ScheduleId { get; set; }
    public Schedule Schedule { get; set; }

    public ICollection<Task> Tasks { get; set; }
    public ICollection<Feedback> Feedbacks { get; set; }
    public ICollection<Statistic> Statistics { get; set; }
}