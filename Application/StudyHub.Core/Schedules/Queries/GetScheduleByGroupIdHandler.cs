using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Schedules.Queries
{
    public record GetScheduleByGroupIdRequest(Guid id): IRequest<ScheduleDto?>;

    public class GetScheduleByGroupIdHandler : IRequestHandler<GetScheduleByGroupIdRequest, ScheduleDto?>
    {
        private readonly IScheduleRepository _repo;

        public GetScheduleByGroupIdHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<ScheduleDto?> Handle(GetScheduleByGroupIdRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetByGroupIdAsync(request.id);
        }
    }
}
