namespace StudyHub.Core.Users.Interfaces;

public interface IUserAuthRepository
{
    Task SignInByEmailAsync(string email, bool isPersistent);
    Task SignOutAsync();
}