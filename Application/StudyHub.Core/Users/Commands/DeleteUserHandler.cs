using MediatR;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public class DeleteUserCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    
    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Guid> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.Delete(request.UserId);
        return request.UserId;
    }
}