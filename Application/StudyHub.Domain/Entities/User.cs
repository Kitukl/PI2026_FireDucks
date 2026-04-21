using Microsoft.AspNetCore.Identity;

namespace StudyHub.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime LastActivity { get; set; }
    public string? MicrosoftId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public bool IsNotified { get; set; }
    public string? PhotoUrl { get; set; }

    public Group? Group { get; set; }

    public Reminder Reminder { get; set; } = new()
    {
        ReminderOffset = 7,
        TimeType = TimeType.Day
    };
    
    public ICollection<Task>? Tasks { get; set; }
    public ICollection<Feedback>? Feedbacks { get; set; }
    public ICollection<Statistic>? Statistics { get; set; }
    public ICollection<UserSession>? Sessions { get; set; }
}
