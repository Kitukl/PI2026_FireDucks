using StudyHub.Domain.Enums;

namespace Application.Models;

public class TaskBoardTaskCardViewModel
{
    public Guid Id { get; set; }
    public Guid? SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TaskCode { get; set; } = string.Empty;
    public bool IsGroupTask { get; set; }
    public Status Status { get; set; }
    public DateTime Deadline { get; set; }
}
