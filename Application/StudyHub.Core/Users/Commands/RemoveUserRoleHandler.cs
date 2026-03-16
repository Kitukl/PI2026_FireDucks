using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public record RemoveUserRoleCommand(UserRoleUpdateDto User) : IRequest;

public class RemoveUserRoleHandler : IRequestHandler<RemoveUserRoleCommand>
{
    private readonly IUserRepository userRepository;
    
    public RemoveUserRoleHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
       await userRepository.RemoveRole(request.User);
    }
}