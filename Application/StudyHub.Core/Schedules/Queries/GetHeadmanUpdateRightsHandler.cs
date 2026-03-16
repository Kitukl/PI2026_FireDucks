using MediatR;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Schedules.Queries
{
    public record GetHeadmanUpdateRightsRequest(Guid id): IRequest<bool>;

    public class GetHeadmanUpdateRightsHandler: IRequestHandler<GetHeadmanUpdateRightsRequest, bool>
    {
        private readonly IScheduleRepository _repo;

        public GetHeadmanUpdateRightsHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(GetHeadmanUpdateRightsRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetHeadmanUpdateRights(request.id);
        }
    }
}
