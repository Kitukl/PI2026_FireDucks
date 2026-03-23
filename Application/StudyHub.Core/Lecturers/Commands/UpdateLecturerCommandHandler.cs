using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Lecturers.Commands
{
    public record UpdateLecturerRequest(LecturerDtoRequest lecturer) : IRequest;

    public class UpdateLecturerCommandHandler : IRequestHandler<UpdateLecturerRequest>
    {
        private readonly ILecturerRepository _repo;

        public UpdateLecturerCommandHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateLecturerRequest request, CancellationToken cancellationToken)
        {
            var lecturer = new Lecturer
            {
                Id = request.lecturer.Id == Guid.Empty ? Guid.NewGuid() : request.lecturer.Id,
                Name = request.lecturer.Name,
                Surname = request.lecturer.Surname,
                Lessons = request.lecturer.Lessons.Select(l => new Lesson
                {
                    Id = l.Id, // == Guid.Empty ? Guid.NewGuid() : l.Id,
                    Day = l.Day,
                    LessonType = l.LessonType,
                    Lecturers = l.Lecturers.Select(x => new Lecturer
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Surname = x.Surname,
                    }).ToList(),
                    Subject = new Subject { Id = l.Subject.Id, Name = l.Subject.Name },
                    LessonsSlot = new LessonsSlot
                    {
                        Id = l.LessonSlot.Id,
                        StartTime = l.LessonSlot.StartTime,
                        EndTime = l.LessonSlot.EndTime
                    }
                }).ToList()
            };

            await _repo.UpdateLecturer(lecturer);
        }
    }
}