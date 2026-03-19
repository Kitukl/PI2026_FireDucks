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
    public record AddScheduleRequest(ScheduleDto dto): IRequest;

    public class AddScheduleHandler: IRequestHandler<AddScheduleRequest>
    {
        private readonly IScheduleRepository _repo;

        public AddScheduleHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddScheduleRequest request,  CancellationToken cancellationToken)
        {
            await _repo.AddSchedule(request.dto);
        }
    }
}
