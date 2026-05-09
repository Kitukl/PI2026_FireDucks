namespace StudyHub.Domain.Entities;

public class UserSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime EntryTimeUtc { get; set; }
    public DateTime LastSeenUtc { get; set; }
    public DateTime? ExitTimeUtc { get; set; }
    public bool IsClosed { get; set; }
    public int DurationSeconds { get; set; }

    public User User { get; set; } = null!;
}
