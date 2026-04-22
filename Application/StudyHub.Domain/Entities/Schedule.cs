namespace StudyHub.Domain.Entities;

public class Schedule
{
    public Guid Id { get; set; }
    public uint UpdateInterval { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsAutoUpdate { get; set; }
    public bool CanLeaderUpdate { get; set; }
    public Group Group { get; set; }
    public Guid GroupId { get; set; }
    
    public ICollection<Lesson> Lessons { get; set; }
}