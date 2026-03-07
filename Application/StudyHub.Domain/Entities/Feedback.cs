namespace StudyHub.Domain.Entitties;

public class Feedback
{
    public Guid Id { get; set; }
    
    public string Type { get; set; }
    public string Category { get; set; }
    public string CreatorFullname { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ResolvedAt { get; set; }
    public string Status { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
