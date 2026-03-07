namespace StudyHub.Domain.Entitties;

public class Schedule
{
    public Guid Id { get; set; }
    public TimeSpan UpdateInterval { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsAutoUpdate { get; set; }
    public bool CanHeadmanUpdate { get; set; }

    public ICollection<Lesson> Lessons { get; set; }
    public ICollection<User> Users { get; set; }
}