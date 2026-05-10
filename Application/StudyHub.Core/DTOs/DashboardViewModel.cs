using StudyHub.Core.DTOs;

namespace Application.Models;

public class DashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public List<TaskBoardTaskCardViewModel> QuickTasks { get; set; } = [];
    public LessonDto? NextLesson { get; set; }
    public string NextLessonDayLabel { get; set; } = "Today";
}
