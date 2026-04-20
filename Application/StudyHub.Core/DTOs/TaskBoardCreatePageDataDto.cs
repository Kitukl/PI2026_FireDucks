using Application.Models;

namespace StudyHub.Core.DTOs;

public class TaskBoardCreatePageDataDto
{
    public TaskBoardPageViewModel Board { get; set; } = new();
    public List<SubjectOptionDto> Subjects { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? SubjectId { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsGroupTask { get; set; }
}
