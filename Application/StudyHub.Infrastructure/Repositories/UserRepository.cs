using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Entities.enums;
using Task = System.Threading.Tasks.Task;


namespace StudyHub.Infrastructure.Repositories;

public class UserRepository(SDbContext context,UserManager<User> userManager) : IUserRepository
{
    public async Task<Dictionary<string, int>> GetUsersCountByRoleAsync()
    {
        return await context.UserRoles
            .Join(context.Roles, 
                ur => ur.RoleId, 
                r => r.Id, 
                (ur, r) => r.Name)
            .GroupBy(roleName => roleName)
            .Select(g => new { RoleName = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleName, x => x.Count);
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        return await context.Users
            .Include(u => u.Group)
            .AsNoTracking()
            .Select(u => new UserDto 
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                GroupName = u.Group.Name,
                Roles = userManager.GetRolesAsync(u).Result.ToList()
            })
            .ToListAsync();
    }

    public async Task Delete(Guid userId)
    {
        await context.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();
    }
    
    public async Task Update(UserUpdateDto userUpdateDto)
    {
        var user = await context.Users.FirstAsync(u => u.Id == userUpdateDto.Id);

        user.Name = userUpdateDto.Name;
        user.Surname = userUpdateDto.Surname;
        user.PhotoUrl = user.PhotoUrl;
        
        var group = await context.Groups
            .FirstOrDefaultAsync(g => g.Name == userUpdateDto.GroupName) ?? new Group();

        user.Group = group;
        
        await context.SaveChangesAsync();
    }
    
    public async Task AddRole(UserRoleUpdateDto userRoleUpdateDto)
    {
        var user = await context.Users.FirstAsync(u => u.Id == userRoleUpdateDto.Id); 
        
        await userManager.AddToRoleAsync(user, userRoleUpdateDto.Role.ToString());
    }
    
    public async Task RemoveRole(UserRoleUpdateDto userRoleUpdateDto)
    {
        var user = await context.Users.FirstAsync(u => u.Id == userRoleUpdateDto.Id);

        await userManager.RemoveFromRoleAsync(user, userRoleUpdateDto.Role.ToString());
    }

    public async Task<UserDto> GetUserById(Guid requestId)
    {
        var user =  await context.Users.FirstAsync(u => u.Id == requestId);
        
        return new UserDto()
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            GroupName = user.Group.Name,
            Roles = userManager.GetRolesAsync(user).Result.ToList(),
        };
    }
}