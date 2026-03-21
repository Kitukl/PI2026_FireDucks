using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

public record GetAllSchedulesRequest : IRequest<List<ScheduleDto>>;

public class GetAllSchedulesQueryHandler : IRequestHandler<GetAllSchedulesRequest, List<ScheduleDto>>
{
    private readonly IScheduleRepository _repo;

    public GetAllSchedulesQueryHandler(IScheduleRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ScheduleDto>> Handle(GetAllSchedulesRequest request, CancellationToken cancellationToken)
    {
        return (await _repo.GetAll())
            .Select(x => new ScheduleDto
            {
                Id = x.Id,
                Group = new GroupDto
                {
                    Id = x.Group.Id,
                    Name = x.Group.Name,
                },
                IsAutoUpdate = x.IsAutoUpdate,
                UpdateAt = x.UpdatedAt,
                HeadmanUpdate = x.CanHeadmanUpdate,
                Lessons = x.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Day = l.Day,
                    LessonType = l.LessonType,
                    Lecturers = l.Lecturers.Select(lec => new LecturerDtoResponse
                    {
                        Id = lec.Id,
                        Name = lec.Name,
                        Surname = lec.Surname
                    }).ToList(),
                    Subject = new SubjectDto { Id = l.Subject.Id, Name = l.Subject.Name },
                    LessonSlot = new LessonSlotDto
                    {
                        Id = l.LessonsSlot.Id,
                        StartTime = l.LessonsSlot.StartTime,
                        EndTime = l.LessonsSlot.EndTime
                    }
                }).ToList()
            }).ToList();
    }
}
