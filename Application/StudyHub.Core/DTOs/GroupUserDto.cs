namespace StudyHub.Core.DTOs;

public class GroupUserDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
}
