using StudyHub.Domain.Entities;

namespace StudyHub.Core.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public List<string> Roles { get; set; }
    public string GroupName { get; set; }
}