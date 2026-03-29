using MediatR;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Users.Commands;

public class UpdateUserCommand : IRequest<User>
{
    public Guid Id { get; set; }
    public string GroupName { get; set; }
}

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.Id);
        if (user == null) throw new Exception("User not found");
        
        var group = await _groupRepository.GetGroupByNameAsync(request.GroupName);

        user.Group = group;
        
        await _userRepository.Update(user);

        return user;
    }
}