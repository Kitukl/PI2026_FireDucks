using StudyHub.Domain.Entities;
using Application.Models;

namespace StudyHub.Core.DTOs;

public class TaskBoardViewTaskPageDataDto
{
    public TaskBoardPageViewModel Board { get; set; } = new();
    public TaskBoardTaskCardViewModel? SelectedTask { get; set; }
    public List<Comment> Comments { get; set; } = new();
    public string CurrentUserFullName { get; set; } = string.Empty;
}
