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
    public record DeleteScheduleForGroupRequest(Guid id) : IRequest;

    public class DeleteScheduleForGroupHandler : IRequestHandler<DeleteScheduleForGroupRequest>
    {
        private readonly IScheduleRepository _repo;

        public DeleteScheduleForGroupHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteScheduleForGroupRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteScheduleForGroup(request.id);
        }
    }
}