using MediatR;
using StudyHub.Core.DTOs;

namespace StudyHub.Core.Group.Queries;

public class GetGroupQuery : IRequest<GroupDto>
{
    public Guid Id { get; set; }
}

public class GetGroupQueryHandler : IRequestHandler<GetGroupQuery, GroupDto>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    public async Task<GroupDto> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupByIdAsync(request.Id);
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name
        };
    }
}