using MediatR;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Users.Commands;

public class UpdateUserCommand : IRequest<User>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Photo { get; set; }
    public string Surname { get; set; }
    public string GroupName { get; set; }
}

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public UpdateUserHandler(IUserRepository userRepository, IGroupRepository groupRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupByNameAsync(request.GroupName);

        var user = new User
        {
            Id = request.Id,
            Name = request.Name,
            PhotoUrl = request.Photo,
            Surname = request.Surname,
            Group = group
        };
        
        await _userRepository.Update(user);

        return user;
    }
}