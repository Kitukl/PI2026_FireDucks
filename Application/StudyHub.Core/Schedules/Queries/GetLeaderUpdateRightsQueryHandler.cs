using MediatR;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.Core.Schedules.Queries
{
    public record GetLeaderUpdateRightsRequest(Guid id): IRequest<bool>;

    public class GetLeaderUpdateRightsQueryHandler: IRequestHandler<GetLeaderUpdateRightsRequest, bool>
    {
        private readonly IScheduleRepository _repo;

        public GetLeaderUpdateRightsQueryHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(GetLeaderUpdateRightsRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetLeaderUpdateRights(request.id);
        }
    }
}
