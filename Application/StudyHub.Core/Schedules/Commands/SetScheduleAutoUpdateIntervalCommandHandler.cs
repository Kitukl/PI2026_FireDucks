using MediatR;
using StudyHub.Core.Schedules.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record SetScheduleAutoUpdateIntervalRequest(uint interval) : IRequest;

    public class SetScheduleAutoUpdateIntervalCommandHandler : IRequestHandler<SetScheduleAutoUpdateIntervalRequest>
    {
        private readonly IScheduleRepository _repo;

        public SetScheduleAutoUpdateIntervalCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(SetScheduleAutoUpdateIntervalRequest request, CancellationToken cancellationToken)
        {
            await _repo.SetScheduleAutoUpdateInterval(request.interval);
        }
    }
}