using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.Core.Schedules.Queries
{
    public record GetLessonsByDayRequest(ScheduleDayDto sdd): IRequest<List<LessonDto>>;

    public class GetLessonsByDayQueryHandler: IRequestHandler<GetLessonsByDayRequest, List<LessonDto>>
    {
        private readonly IScheduleRepository _repo;

        public GetLessonsByDayQueryHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LessonDto>> Handle(GetLessonsByDayRequest request, CancellationToken cancellationToken)
        {
            return (await _repo.GetLessonsByDay(request.sdd.Id, request.sdd.Day))
                .Select(x => new LessonDto
                {
                    Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                    Day = x.Day,
                    LessonType = x.LessonType,
                    Lecturers = x.Lecturers.Select(x => new LecturerDtoResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Surname = x.Surname
                    }).ToList(),
                    Subject = new SubjectDto { Id = x.Subject.Id, Name = x.Subject.Name },
                    LessonSlot = new LessonSlotDto
                    {
                        Id = x.Id,
                        StartTime = x.LessonsSlot.StartTime,
                        EndTime = x.LessonsSlot.EndTime
                    }
                }).ToList();
        }
    }
}
