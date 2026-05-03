using MediatR;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public class RegisterUserCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string GroupName { get; set; }
    public string ProviderName { get; set; }
    public string MicrosoftId { get; set; }
    public bool SignInAfterRegister { get; set; }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAuthRepository _userAuthRepository;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUserAuthRepository userAuthRepository)
    {
        _userRepository = userRepository;
        _userAuthRepository = userAuthRepository;
    }
    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersAsync();
        var existingUser = users.FirstOrDefault(u => u.Email == request.Email);

        if (existingUser != null)
        {
            if (request.SignInAfterRegister && !string.IsNullOrWhiteSpace(request.Email))
            {
                await _userAuthRepository.SignInByEmailAsync(request.Email, true);
            }

            return true;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
            Surname = request.Surname,
            MicrosoftId = request.MicrosoftId,
            LastActivity = DateTime.UtcNow,
            EmailConfirmed = true
        };

        await _userRepository.CreateUser(user);
        await _userRepository.AddExternalLogin(user, request.ProviderName, request.MicrosoftId);
        await _userRepository.AddRole(Role.Student, user.Id);

        if (request.SignInAfterRegister && !string.IsNullOrWhiteSpace(request.Email))
        {
            await _userAuthRepository.SignInByEmailAsync(request.Email, true);
        }

        return true;
    }
}