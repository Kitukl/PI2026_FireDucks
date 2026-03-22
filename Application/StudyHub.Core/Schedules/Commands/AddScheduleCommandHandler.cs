using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Schedules.Commands
{
    public record AddScheduleRequest(ScheduleDto dto): IRequest;

    public class AddScheduleCommandHandler: IRequestHandler<AddScheduleRequest>
    {
        private readonly IScheduleRepository _repo;

        public AddScheduleCommandHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddScheduleRequest request,  CancellationToken cancellationToken)
        {
            var schedule = new Schedule
            {
                Id = request.dto.Id == Guid.Empty ? Guid.NewGuid() : request.dto.Id,
                UpdatedAt = request.dto.UpdateAt,
                CanHeadmanUpdate = request.dto.HeadmanUpdate,
                IsAutoUpdate = request.dto.IsAutoUpdate,
                Group = new Group { Id = request.dto.Group.Id, Name = request.dto.Group.Name },
                Lessons = request.dto.Lessons?.Select(x => new Lesson
                {
                    Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                    Day = x.Day,
                    LessonType = x.LessonType,
                    Subject = new Subject
                    {
                        Id = x.Subject.Id == Guid.Empty ? Guid.NewGuid() : x.Subject.Id,
                        Name = x.Subject.Name
                    },
                    LessonsSlot = new LessonsSlot
                    {
                        Id = x.LessonSlot.Id == Guid.Empty ? Guid.NewGuid() : x.LessonSlot.Id,
                        StartTime = x.LessonSlot.StartTime,
                        EndTime = x.LessonSlot.EndTime
                    },
                    Lecturers = x.Lecturers?.Select(l => new Lecturer
                    {
                        Id = l.Id == Guid.Empty ? Guid.NewGuid() : l.Id,
                        Name = l.Name,
                        Surname = l.Surname,
                    }).ToList() ?? new List<Lecturer>()
                }).ToList() ?? new List<Lesson>()
            };

            await _repo.AddSchedule(schedule);
        }
    }
}
