using StudyHub.Domain.Entities.enums;

namespace StudyHub.Core.DTOs;

public class UserRoleUpdateDto
{
    public Guid Id { get; set; }
    public Role Role { get; set; }
}