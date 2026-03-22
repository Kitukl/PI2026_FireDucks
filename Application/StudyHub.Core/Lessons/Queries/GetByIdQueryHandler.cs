using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;

namespace StudyHub.Core.Lessons.Queries
{
    public record GetLessonByIdRequest(Guid id) : IRequest<LessonDto?>;

    public class GetByIdQueryHandler : IRequestHandler<GetLessonByIdRequest, LessonDto?>
    {
        private readonly ILessonRepository _repo;

        public GetByIdQueryHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task<LessonDto?> Handle(GetLessonByIdRequest request, CancellationToken cancellationToken)
        {
            var lesson = await _repo.GetById(request.id);

            if (lesson != null) 
            {
                var lessonDto = new LessonDto
                {
                    Id = lesson.Id,
                    Day = lesson.Day,
                    LessonType = lesson.LessonType,
                    Subject = new SubjectDto { Id = lesson.Subject.Id, Name = lesson.Subject.Name },
                    Lecturers = lesson.Lecturers.Select(x => new LecturerDtoResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Surname = x.Surname
                    }).ToList(),
                    LessonSlot = new LessonSlotDto
                    {
                        Id = lesson.LessonsSlot.Id,
                        StartTime = lesson.LessonsSlot.StartTime,
                        EndTime = lesson.LessonsSlot.EndTime,
                    }
                };

                return lessonDto;
            }

            return null;
        }
    }
}