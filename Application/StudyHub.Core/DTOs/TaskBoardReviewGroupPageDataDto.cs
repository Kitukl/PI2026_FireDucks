using Application.Models;

namespace StudyHub.Core.DTOs;

public class TaskBoardReviewGroupPageDataDto
{
    public TaskBoardPageViewModel Board { get; set; } = new();
    public string GroupName { get; set; } = "Group";
    public List<GroupUserDto> GroupUsers { get; set; } = new();
    public List<GroupUserDto> UnassignedUsers { get; set; } = new();
    public string? ResourceUrl { get; set; }
}
