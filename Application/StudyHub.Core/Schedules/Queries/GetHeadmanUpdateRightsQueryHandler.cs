using MediatR;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.Core.Schedules.Queries
{
    public record GetHeadmanUpdateRightsRequest(Guid id): IRequest<bool>;

    public class GetHeadmanUpdateRightsQueryHandler: IRequestHandler<GetHeadmanUpdateRightsRequest, bool>
    {
        private readonly IScheduleRepository _repo;

        public GetHeadmanUpdateRightsQueryHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(GetHeadmanUpdateRightsRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetHeadmanUpdateRights(request.id);
        }
    }
}
