using MediatR;
using StudyHub.Core.Schedules.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record DeleteAllRequest : IRequest;

    public class DeleteAllCommandHandler : IRequestHandler<DeleteAllRequest>
    {
        private readonly IScheduleRepository _repo;

        public DeleteAllCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteAllRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteAllAsync();
        }
    }
}