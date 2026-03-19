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
    public record DeleteAllRequest : IRequest;

    public class DeleteAllHandler : IRequestHandler<DeleteAllRequest>
    {
        private readonly IScheduleRepository _repo;

        public DeleteAllHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteAllRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteAllAsync();
        }
    }
}