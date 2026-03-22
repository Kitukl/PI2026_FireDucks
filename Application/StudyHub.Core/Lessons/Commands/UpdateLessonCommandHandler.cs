using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Lessons.Commands
{
    public record UpdateLessonRequest(LessonDto lesson) : IRequest;

    public class UpdateLessonCommandHandler : IRequestHandler<UpdateLessonRequest>
    {
        private readonly ILessonRepository _repo;

        public UpdateLessonCommandHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateLessonRequest request, CancellationToken cancellationToken)
        {
            var lesson = new Lesson
            {
                Id = request.lesson.Id == Guid.Empty ? Guid.NewGuid() : request.lesson.Id,
                Day = request.lesson.Day,
                LessonType = request.lesson.LessonType,
                Lecturers = request.lesson.Lecturers.Select(x => new Lecturer
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                }).ToList(),
                Subject = new Subject { Id = request.lesson.Subject.Id, Name = request.lesson.Subject.Name },
                LessonsSlot = new LessonsSlot
                {
                    Id = request.lesson.Id,
                    StartTime = request.lesson.LessonSlot.StartTime,
                    EndTime = request.lesson.LessonSlot.EndTime
                }

            };

            await _repo.UpdateLesson(lesson);
        }
    }
}