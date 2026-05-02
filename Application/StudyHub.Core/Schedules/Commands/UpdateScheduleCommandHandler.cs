using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;
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
            var scheduleUpdate = new Schedule
            {
                Id = request.dto.Id,
                IsAutoUpdate = request.dto.IsAutoUpdate,
                CanLeaderUpdate = request.dto.LeaderUpdate,
                UpdatedAt = DateTime.UtcNow,
                Lessons = request.dto.Lessons?
                    .Where(l => l != null)
                    .Select(l => new Lesson { Id = l.Id })
                    .ToList() ?? new List<Lesson>()
            };

            await _repo.UpdateScheduleAsync(scheduleUpdate);
        }
    }
}
