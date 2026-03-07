namespace StudyHub.Domain.Entitties;

public class Task
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string  Subject { get; set; }
    public bool IsGroupTask { get; set; }
    public string Status { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public ICollection<Statistic> Statistics { get; set; }
    public ICollection<Comment> Comments { get; set; }
}