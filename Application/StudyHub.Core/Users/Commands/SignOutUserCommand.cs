using MediatR;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public class SignOutUserCommand : IRequest
{
}

public class SignOutUserCommandHandler : IRequestHandler<SignOutUserCommand>
{
    private readonly IUserAuthRepository _userAuthRepository;

    public SignOutUserCommandHandler(IUserAuthRepository userAuthRepository)
    {
        _userAuthRepository = userAuthRepository;
    }

    public async Task Handle(SignOutUserCommand request, CancellationToken cancellationToken)
    {
        await _userAuthRepository.SignOutAsync();
    }
}