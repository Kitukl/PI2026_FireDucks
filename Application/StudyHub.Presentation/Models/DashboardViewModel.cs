namespace Application.Models;

public class DashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public List<TaskBoardTaskCardViewModel> QuickTasks { get; set; } = [];
}
