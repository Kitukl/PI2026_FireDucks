using MediatR;
using StudyHub.Core.Group;
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
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }
    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersAsync();
        var existingUser = users.FirstOrDefault(u => u.Email == request.Email);

        if (existingUser != null)
        {
            var existingRoles = await _userRepository.GetRolesByUser(existingUser);
            if (!existingRoles.Any(role => string.Equals(role, nameof(Role.Student), StringComparison.OrdinalIgnoreCase)))
            {
                await _userRepository.AddRole(Role.Student, existingUser.Id);
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

        return true;
    }
}