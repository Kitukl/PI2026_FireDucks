using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public record AddUserRoleCommand(UserRoleUpdateDto UserRole) : IRequest;

public class AddUserRoleHandler : IRequestHandler<AddUserRoleCommand>
{
    private readonly IUserRepository userRepository;
    
    public AddUserRoleHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    public async Task Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    { 
        await userRepository.AddRole(request.UserRole);
    }
}