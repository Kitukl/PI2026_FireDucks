using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record UpdateScheduleRequest(ScheduleDto dto) : IRequest;

    public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleRequest>
    {
        private readonly IScheduleRepository _repo;

        public UpdateScheduleCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateScheduleRequest request, CancellationToken cancellationToken)
        {
            var schedule = await _repo.GetByGroupIdAsync(request.dto.Id);

            if (schedule != null)
            {
                await _repo.UpdateScheduleAsync(schedule);
            }
        }
    }
}