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
    public record SetScheduleAutoUpdateRequest : IRequest;

    public class SetScheduleAutoUpdateHandler : IRequestHandler<SetScheduleAutoUpdateRequest>
    {
        private readonly IScheduleRepository _repo;

        public SetScheduleAutoUpdateHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(SetScheduleAutoUpdateRequest request, CancellationToken cancellationToken)
        {
            await _repo.SetScheduleAutoUpdate();
        }
    }
}