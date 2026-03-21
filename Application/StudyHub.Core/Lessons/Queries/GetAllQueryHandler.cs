using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;


namespace StudyHub.Core.Lessons.Queries
{
    public record GetAllRequest : IRequest<List<LessonDto>>;

    public class GetAllQueryHandler : IRequestHandler<GetAllRequest, List<LessonDto>>
    {
        private readonly ILessonRepository _repo;

        public GetAllQueryHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LessonDto>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return (await _repo.GetAll())
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    Day = l.Day,
                    LessonType = l.LessonType,
                    Subject = new SubjectDto { Id = l.Subject.Id, Name = l.Subject.Name },
                    Lecturers = l.Lecturers.Select(x => new LecturerDtoResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Surname = x.Surname
                    }).ToList(),
                    LessonSlot = new LessonSlotDto
                    {
                        Id = l.LessonsSlot.Id,
                        StartTime = l.LessonsSlot.StartTime,
                        EndTime = l.LessonsSlot.EndTime,
                    }
                })
                .ToList();
        }
    }
}