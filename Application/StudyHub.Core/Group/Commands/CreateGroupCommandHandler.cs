using MediatR;
using StudyHub.Core.DTOs;

namespace StudyHub.Core.Group.Commands;

public class CreateGroupCommand : IRequest<GroupDto>
{
    public string Name { get; set; }
}

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupDto>
{
    private readonly IGroupRepository _groupRepository;

    public CreateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var result = await _groupRepository.CreateGroupAsync(request.Name);
        return new GroupDto
        {
            Id = result.Id,
            Name = result.Name
        };
    }
}