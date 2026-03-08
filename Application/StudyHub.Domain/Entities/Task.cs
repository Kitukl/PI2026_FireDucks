namespace StudyHub.Domain.Entities;

public class Task
{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public bool IsGroupTask { get; set; }
    public Status Status { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime CreatedAt { get; set; }

    public Subject Subject { get; set; }
    
    public User User { get; set; }

    public ICollection<Statistic> Statistics { get; set; }
    public ICollection<Comment> Comments { get; set; }
}

public enum Status
{
    ToDo,
    InProgress,
    ForReview,
    Done
}