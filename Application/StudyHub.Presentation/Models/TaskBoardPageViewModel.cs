using StudyHub.Domain.Enums;

namespace Application.Models;

public class TaskBoardPageViewModel
{
    public List<TaskBoardTaskCardViewModel> Tasks { get; set; } = [];
    public List<string> Subjects { get; set; } = [];

    public int CountTasks(bool groupOnly, Status status)
    {
        return Tasks.Count(t => t.IsGroupTask == groupOnly && t.Status == status);
    }

    public int CountTasks(Status status)
    {
        return Tasks.Count(t => t.Status == status);
    }
}
