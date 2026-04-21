namespace StudyHub.Core.Admin.DTOs;

public class AdminUserUpdateResultDto
{
    public bool IsSuccess { get; set; }
    public Guid UserId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
