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
    public record UpdateScheduleRequest(ScheduleDto dto) : IRequest;

    public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleRequest>
    {
        private readonly IScheduleRepository _repo;

        public UpdateScheduleHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateScheduleRequest request, CancellationToken cancellationToken)
        {
            await _repo.UpdateScheduleAsync(request.dto);
        }
    }
}