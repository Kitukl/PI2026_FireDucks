using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Schedules.Commands
{
    public record UpdateHeadmanRightsRequest(ScheduleHeadmanRightsUpdateDto dto) : IRequest;

    public class UpdateHeadmanRightsHandler : IRequestHandler<UpdateHeadmanRightsRequest>
    {
        private readonly IScheduleRepository _repo;

        public UpdateHeadmanRightsHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateHeadmanRightsRequest request, CancellationToken cancellationToken)
        {
            await _repo.UpdateHeadmanRights(request.dto);
        }
    }
}