namespace StudyHub.Core.Users.Interfaces;

public interface IUserAuthRepository
{
    Task SignInByEmailAsync(string email, bool isPersistent);
    Task RefreshSignInAsync(Guid userId);
    Task SignOutAsync();
}