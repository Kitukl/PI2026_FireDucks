using StudyHub.Domain.Enums;

namespace StudyHub.Domain.Entities;

public class Feedback
{
    public Guid Id { get; set; }
    
    public FeedbackType FeedbackType { get; set; }
    public Category Category { get; set; }
    
    public string CreatorFullname { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Status Status { get; set; }

    public User User { get; set; }
    public ICollection<Comment>? Comments { get; set; }
}