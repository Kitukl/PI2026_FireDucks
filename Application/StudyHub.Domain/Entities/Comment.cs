namespace StudyHub.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid TaskId { get; set; }
    public Task Task { get; set; }
}