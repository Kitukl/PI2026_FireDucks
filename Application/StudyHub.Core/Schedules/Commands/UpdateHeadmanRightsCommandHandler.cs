using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record UpdateHeadmanRightsRequest(ScheduleHeadmanRightsUpdateDtoRequest dto) : IRequest;

    public class UpdateHeadmanRightsCommandHandler : IRequestHandler<UpdateHeadmanRightsRequest>
    {
        private readonly IScheduleRepository _repo;

        public UpdateHeadmanRightsCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateHeadmanRightsRequest request, CancellationToken cancellationToken)
        {
            var schedule = await _repo.GetByGroupIdAsync(request.dto.Id);

            if (schedule != null)
            {
               await _repo.UpdateHeadmanRights(schedule);
            }
        }
    }
}