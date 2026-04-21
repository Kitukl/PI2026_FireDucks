using Microsoft.AspNetCore.Identity;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Infrastructure.Repositories;

public class UserAuthRepository : IUserAuthRepository
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserAuthRepository(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task SignInByEmailAsync(string email, bool isPersistent)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        await _signInManager.SignInAsync(user, isPersistent);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}