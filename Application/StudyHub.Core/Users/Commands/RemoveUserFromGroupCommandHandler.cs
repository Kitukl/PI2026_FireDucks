using MediatR;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public class RemoveUserFromGroupCommand : IRequest<string>
{
    public Guid UserId { get; set; }
}

public class RemoveUserFromGroupCommandHandler : IRequestHandler<RemoveUserFromGroupCommand, string>
{
    private readonly IUserRepository _repository;

    public RemoveUserFromGroupCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    public async Task<string> Handle(RemoveUserFromGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserById(request.UserId) ?? throw new Exception("user not found");
        user.Group = null;

        await _repository.Update(user);
        
        return user.Name;
    }
}