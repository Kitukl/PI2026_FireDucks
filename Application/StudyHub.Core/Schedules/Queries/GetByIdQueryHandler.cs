using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

public record GetScheduleByIdRequest(Guid id) : IRequest<ScheduleDto?>;

public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdRequest, ScheduleDto?>
{
    private readonly IScheduleRepository _repo;

    public GetScheduleByIdQueryHandler(IScheduleRepository repo)
    {
        _repo = repo;
    }

    public async Task<ScheduleDto?> Handle(GetScheduleByIdRequest request, CancellationToken cancellationToken)
    {
        var schedule = await _repo.GetById(request.id);
        
        if (schedule != null)
        {
            var scheduleDto = new ScheduleDto
            {
                Id = schedule.Id,
                Group = new GroupDto
                {
                    Id = schedule.Group.Id,
                    Name = schedule.Group.Name,
                },
                IsAutoUpdate = schedule.IsAutoUpdate,
                UpdateAt = schedule.UpdatedAt,
                HeadmanUpdate = schedule.CanHeadmanUpdate,
                Lessons = schedule.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Day = l.Day,
                    LessonType = l.LessonType,
                    Lecturers = l.Lecturers.Select(x => new LecturerDtoResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Surname = x.Surname
                    }).ToList(),
                    Subject = new SubjectDto { Id = l.Subject.Id, Name = l.Subject.Name },
                    LessonSlot = new LessonSlotDto
                    {
                        Id = l.LessonsSlot.Id,
                        StartTime = l.LessonsSlot.StartTime,
                        EndTime = l.LessonsSlot.EndTime
                    }
                }).ToList()
            };

            return scheduleDto;
        }

        return null;
    }
}
