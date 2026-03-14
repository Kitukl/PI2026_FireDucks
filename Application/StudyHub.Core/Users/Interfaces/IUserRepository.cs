using StudyHub.Core.DTOs;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Users.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task Delete(Guid userId);
    Task Update(UserUpdateDto userUpdateDto);
    Task AddRole(UserRoleUpdateDto userRoleUpdateDto);
    Task RemoveRole(UserRoleUpdateDto userRoleUpdateDto);

    Task<UserDto> GetUserById(Guid requestId);
}