using MediatR;

namespace StudyHub.Core.Group.Queries;

public class GetAllGroupsQuery : IRequest<List<Domain.Entities.Group>>;

public class GetAllGroupsQueryHanlder : IRequestHandler<GetAllGroupsQuery, List<Domain.Entities.Group>>
{
    private readonly IGroupRepository _groupRepository;

    public GetAllGroupsQueryHanlder(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    public async Task<List<Domain.Entities.Group>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _groupRepository.GetAllGroupsAsync();
        return groups.ToList();
    }
}