using MediatR;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public record DeleteUserCommand(Guid Id) : IRequest;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository userRepository;
    
    public DeleteUserHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        await userRepository.Delete(command.Id);
    }
}