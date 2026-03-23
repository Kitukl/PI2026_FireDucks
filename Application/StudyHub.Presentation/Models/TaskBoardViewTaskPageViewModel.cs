using StudyHub.Domain.Entities;

namespace Application.Models;

public class TaskBoardViewTaskPageViewModel
{
    public TaskBoardPageViewModel Board { get; set; } = new();
    public TaskBoardTaskCardViewModel? SelectedTask { get; set; }
    public List<Comment> Comments { get; set; } = [];
}
