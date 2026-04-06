using MediatR;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public class AddUserToGroupCommand : IRequest<string>
{
    public Guid UserId { get; set; }
    public string GroupName { get; set; }
}

public class AddUserToGroupCommandHandler : IRequestHandler<AddUserToGroupCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public AddUserToGroupCommandHandler(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }
    public async Task<string> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);
        if (string.IsNullOrWhiteSpace(request.GroupName))
        {
            throw new ArgumentException("Group name is required", nameof(request.GroupName));
        }

        var normalizedGroupName = request.GroupName.Trim();
        var group = await _groupRepository.GetGroupByNameAsync(normalizedGroupName)
                    ?? await _groupRepository.CreateGroupAsync(normalizedGroupName);

        user.Group = group;

        await _userRepository.Update(user);
        return group.Name;
    }
}