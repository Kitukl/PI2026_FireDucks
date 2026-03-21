using MediatR;
using StudyHub.Core.Schedules.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record SetScheduleAutoUpdateRequest(bool value) : IRequest;

    public class SetScheduleAutoUpdateCommandHandler : IRequestHandler<SetScheduleAutoUpdateRequest>
    {
        private readonly IScheduleRepository _repo;

        public SetScheduleAutoUpdateCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(SetScheduleAutoUpdateRequest request, CancellationToken cancellationToken)
        {
            await _repo.SetScheduleAutoUpdate(request.value);
        }
    }
}