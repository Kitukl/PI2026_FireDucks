using MediatR;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public class AddUserRoleCommand : IRequest
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
}

public class AddUserRoleCommandHandler : IRequestHandler<AddUserRoleCommand>
{
    private readonly IUserRepository _userRepository;
    
    public AddUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    { 
        await _userRepository.AddRole(request.Role, request.UserId);
    }
}