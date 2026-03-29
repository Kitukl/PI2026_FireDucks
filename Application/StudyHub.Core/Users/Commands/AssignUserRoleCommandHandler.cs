using MediatR;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public class AssignUserRoleCommand : IRequest
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
}

public class AssignUserRoleCommandHandler : IRequestHandler<AssignUserRoleCommand>
{
    private readonly IUserRepository _userRepository;

    public AssignUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        // Deliberately updates role records only for the target user without refreshing current admin sign-in.
        await _userRepository.AddRole(request.Role, request.UserId);
    }
}
