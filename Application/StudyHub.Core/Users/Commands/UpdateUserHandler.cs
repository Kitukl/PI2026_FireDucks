using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public record UpdateUserCommand(UserUpdateDto User): IRequest;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository userRepository;
    
    public UpdateUserHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await userRepository.Update(request.User);
    }
}