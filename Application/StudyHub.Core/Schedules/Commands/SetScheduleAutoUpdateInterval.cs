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
    public record SetScheduleAutoUpdateIntervalRequest(uint interval) : IRequest;

    public class SetScheduleAutoUpdateIntervalHandler : IRequestHandler<SetScheduleAutoUpdateIntervalRequest>
    {
        private readonly IScheduleRepository _repo;

        public SetScheduleAutoUpdateIntervalHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(SetScheduleAutoUpdateIntervalRequest request, CancellationToken cancellationToken)
        {
            await _repo.SetScheduleAutoUpdateInterval(request.interval);
        }
    }
}