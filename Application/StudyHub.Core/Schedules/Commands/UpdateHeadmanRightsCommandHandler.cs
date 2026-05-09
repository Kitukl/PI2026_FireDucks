using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record UpdateLeaderRightsRequest(ScheduleLeaderRightsUpdateDtoRequest dto) : IRequest;

    public class UpdateLeaderRightsCommandHandler : IRequestHandler<UpdateLeaderRightsRequest>
    {
        private readonly IScheduleRepository _repo;

        public UpdateLeaderRightsCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateLeaderRightsRequest request, CancellationToken cancellationToken)
        {
            var schedule = await _repo.GetByGroupIdAsync(request.dto.Id);

            if (schedule != null)
            {
               await _repo.UpdateLeaderRights(schedule);
            }
        }
    }
}