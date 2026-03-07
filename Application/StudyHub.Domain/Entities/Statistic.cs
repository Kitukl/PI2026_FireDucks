namespace StudyHub.Domain.Entitties;

public class Statistic
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public double UserActivityPerMonth { get; set; }
    public int FilesCount { get; set; }

    public ICollection<User> Users { get; set; }
    public ICollection<Task> Tasks { get; set; }
}