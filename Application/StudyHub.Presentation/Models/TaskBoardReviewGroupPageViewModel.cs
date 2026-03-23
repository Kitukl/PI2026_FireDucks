namespace Application.Models;

public class TaskBoardReviewGroupPageViewModel
{
    public TaskBoardPageViewModel Board { get; set; } = new();
    public string GroupName { get; set; } = "Group";
    public List<GroupUserViewModel> GroupUsers { get; set; } = [];
    public List<GroupUserViewModel> UnassignedUsers { get; set; } = [];
}
