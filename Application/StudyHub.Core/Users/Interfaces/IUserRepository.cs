using StudyHub.Core.DTOs;

namespace StudyHub.Core.Users.Interfaces;

public interface IUserRepository
{
    Task<Dictionary<string,int>> GetUsersCountByRoleAsync();
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task Delete(Guid userId);
    Task Update(UserUpdateDto userUpdateDto);
    Task AddRole(UserRoleUpdateDto userRoleUpdateDto);
    Task RemoveRole(UserRoleUpdateDto userRoleUpdateDto);
    Task<UserDto> GetUserById(Guid requestId);
}