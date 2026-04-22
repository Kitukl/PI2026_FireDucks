using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.Core.Schedules.Queries
{
    public record GetScheduleByGroupIdRequest(Guid id): IRequest<ScheduleDto?>;

    public class GetScheduleByGroupIdQueryHandler : IRequestHandler<GetScheduleByGroupIdRequest, ScheduleDto?>
    {
        private readonly IScheduleRepository _repo;

        public GetScheduleByGroupIdQueryHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<ScheduleDto?> Handle(GetScheduleByGroupIdRequest request, CancellationToken cancellationToken)
        {
            var schedule = await _repo.GetByGroupIdAsync(request.id);

            if (schedule != null)
            {
                var scheduleDto = new ScheduleDto
                {
                    Id = schedule.Id,
                    UpdateAt = schedule.UpdatedAt,
                    LeaderUpdate = schedule.CanLeaderUpdate,
                    IsAutoUpdate = schedule.IsAutoUpdate,
                    Group = new GroupDto { Id = schedule.Group.Id, Name = schedule.Group.Name },
                    Lessons = schedule.Lessons.Select(x => new LessonDto
                    {
                        Id = x.Id,
                        Day = x.Day,
                        LessonType = x.LessonType,
                        Subject = new SubjectDto
                        {
                            Id = x.Subject.Id,
                            Name = x.Subject.Name
                        },
                        LessonSlot = new LessonSlotDto
                        {
                            Id = x.LessonsSlot.Id,
                            StartTime = x.LessonsSlot.StartTime,
                            EndTime = x.LessonsSlot.EndTime
                        },
                        Lecturers = x.Lecturers?.Select(l => new LecturerDtoResponse
                        {
                            Id = l.Id == Guid.Empty ? Guid.NewGuid() : l.Id,
                            Name = l.Name,
                            Surname = l.Surname,
                        }).ToList() ?? new List<LecturerDtoResponse>(),
                        Room = x.Room
                    }).ToList()
                };

                return scheduleDto;
            }

            return null;
        }
    }
}
