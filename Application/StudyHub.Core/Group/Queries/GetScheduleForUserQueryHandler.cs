using Application.Models;
using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Group.Queries;

public class GetScheduleForUserQuery : IRequest<ScheduleViewModel>
{
    public string UserId { get; set; }
}

public class GetScheduleForUserQueryHandler : IRequestHandler<GetScheduleForUserQuery, ScheduleViewModel>
{
    private readonly IUserRepository _userRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public GetScheduleForUserQueryHandler(IUserRepository userRepository, IScheduleRepository scheduleRepository)
    {
        _userRepository = userRepository;
        _scheduleRepository = scheduleRepository;
    }
    
    public async Task<ScheduleViewModel?> Handle(GetScheduleForUserQuery request, CancellationToken cancellationToken)
    {
        var normalizedId = Guid.Parse(request.UserId);
        var currentUser = await _userRepository.GetUserById(normalizedId);

        if (currentUser.Group == null)
        {
            return null;
        }

        var schedule = await _scheduleRepository.GetByGroupIdAsync(currentUser.Group.Id);

        var isLeader = await _userRepository.IsLeader(normalizedId);

        var response = new ScheduleViewModel
        {
            GroupId = currentUser.Group.Id,
            GroupName = currentUser.Group.Name,
            IsLeader = isLeader,
            CanLeaderUpdate = schedule?.CanHeadmanUpdate ?? false
        };

        if (schedule != null && schedule.Lessons.Any())
        {
            response.UniqueSlots = schedule.Lessons
                .Select(l => new LessonSlotDto
                {
                    Id = l.LessonsSlot.Id,
                    StartTime = l.LessonsSlot.StartTime,
                    EndTime = l.LessonsSlot.EndTime
                })
                .GroupBy(s => s.Id)
                .Select(g => g.First())
                .OrderBy(s => s.StartTime)
                .ToList();

            response.Days = new List<Domain.Enums.DayOfWeek>
            {
                (Domain.Enums.DayOfWeek)DayOfWeek.Monday, (Domain.Enums.DayOfWeek)DayOfWeek.Tuesday, (Domain.Enums.DayOfWeek)DayOfWeek.Wednesday,
                (Domain.Enums.DayOfWeek)DayOfWeek.Thursday, (Domain.Enums.DayOfWeek)DayOfWeek.Friday
            };

            foreach (var lesson in schedule.Lessons)
            {
                var key = $"{(int)lesson.Day}-{lesson.LessonsSlot.Id}";
                if (!response.Grid.ContainsKey(key))
                {
                    response.Grid[key] = new List<LessonDto>();
                }
                response.Grid[key].Add(new LessonDto
                {
                    Day = lesson.Day,
                    Id = lesson.Id,
                    Lecturers = lesson.Lecturers.Select(l => new LecturerDtoResponse
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Surname = l.Surname
                    }).ToList(),
                    LessonSlot = new LessonSlotDto
                    {
                        Id = lesson.LessonsSlot.Id,
                        StartTime = lesson.LessonsSlot.StartTime,
                        EndTime = lesson.LessonsSlot.EndTime
                    },
                    LessonType = lesson.LessonType,
                    Room = lesson.Room,
                    Subject = new SubjectDto
                    {
                        Id = lesson.Subject.Id,
                        Name = lesson.Subject.Name
                    }
                });
            }
        }

        return response;
    }
}