using MediatR;

namespace StudyHub.Core.Group.Queries;

public class GetAllGroupsQuery : IRequest<List<Domain.Entities.Group>>;

public class GetAllGroupsQueryHanlder : IRequestHandler<GetAllGroupsQuery, IEnumerable<Domain.Entities.Group>>
{
    private readonly IGroupRepository _groupRepository;

    public GetAllGroupsQueryHanlder(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    public async Task<IEnumerable<Domain.Entities.Group>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _groupRepository.GetAllGroupsAsync();
    }
}