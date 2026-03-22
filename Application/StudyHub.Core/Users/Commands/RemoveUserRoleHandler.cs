using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public record RemoveUserRoleCommand : IRequest
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
}

public class RemoveUserRoleHandler : IRequestHandler<RemoveUserRoleCommand>
{
    private readonly IUserRepository _userRepository;
    
    public RemoveUserRoleHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
       await _userRepository.RemoveRole(request.Role, request.UserId);
    }
}