using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Users.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<bool> IsHeadman(Guid id);
    Task<List<string>> GetAllEmailsAsync();
    Task Delete(Guid userId);
    Task<User> Update(User userUpdateDto);
    Task AddRole(Role userRole, Guid userId);
    Task RemoveRole(Role userRole, Guid userId);
    Task<User> GetUserById(Guid requestId);
    Task<User> CreateUser(User user);
    Task<List<string>> GetRolesByUser(User user);
    Task AddExternalLogin(User user, string provider, string providerKey);
}