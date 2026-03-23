namespace Application.Models;

public class GroupUserViewModel
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
}
