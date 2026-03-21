using MediatR;
using StudyHub.Core.Schedules.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record DeleteScheduleForGroupRequest(Guid id) : IRequest;

    public class DeleteScheduleForGroupCommandHandler : IRequestHandler<DeleteScheduleForGroupRequest>
    {
        private readonly IScheduleRepository _repo;

        public DeleteScheduleForGroupCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteScheduleForGroupRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteScheduleForGroup(request.id);
        }
    }
}