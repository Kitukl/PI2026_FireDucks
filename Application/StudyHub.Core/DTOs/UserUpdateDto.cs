namespace StudyHub.Core.DTOs;

public class UserUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Photo { get; set; }
    public string Surname { get; set; }
    public string GroupName { get; set; }
}
