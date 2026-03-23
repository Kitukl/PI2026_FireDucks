using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using Task = System.Threading.Tasks.Task;


namespace StudyHub.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserRepository(SDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.Include(u=>u.Group).ToListAsync();
    }

    public async Task Delete(Guid userId)
    {
        await _context.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();
    }
    
    public async Task<User> Update(User user)
    {
        _context.Update(user);
        
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task AddRole(Role userRole, Guid userId)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == userId); 
        await _userManager.AddToRoleAsync(user, userRole.ToString());
    }

    public async Task RemoveRole(Role userRole, Guid userId)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == userId);
        await _userManager.RemoveFromRoleAsync(user, userRole.ToString());
    }

    public async Task<User> GetUserById(Guid requestId)
    {
        return await _userManager.FindByIdAsync(requestId.ToString()) ?? throw new Exception("User not found");
    }
    public async Task<User> CreateUser(User user)
    {
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded) throw new Exception("Помилка створення");
        return user;
    }

    public async Task<List<string>> GetRolesByUser(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
    
    public async Task AddExternalLogin(User user, string provider, string providerKey)
    {
        var info = new UserLoginInfo(provider, providerKey, provider);
        var result = await _userManager.AddLoginAsync(user, info);
    
        if (!result.Succeeded)
        {
            throw new Exception("Не вдалося прив'язати зовнішній акаунт");
        }
    }
}