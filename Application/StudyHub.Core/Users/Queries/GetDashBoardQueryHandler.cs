using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StudyHub.Core.Common;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public class GetDashBoardQuery : IRequest<DashboardViewModel>
{
    public Guid UserId { get; set; }
}

public class GetDashBoardQueryHandler : IRequestHandler<GetDashBoardQuery, DashboardViewModel>
{
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ILogger<GetDashBoardQueryHandler> _logger;

    public GetDashBoardQueryHandler(IUserRepository userRepository, ITaskRepository taskRepository, IScheduleRepository scheduleRepository, ILogger<GetDashBoardQueryHandler> logger)
    {
        _userRepository = userRepository;
        _taskRepository = taskRepository;
        _scheduleRepository = scheduleRepository;
        _logger = logger;
    }
    public async Task<DashboardViewModel> Handle(GetDashBoardQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);
        var fullName = $"{user.Name} {user.Surname}".Trim();
        
        var tasks = await _taskRepository.GetTasksAsync();
        tasks = tasks.Where(task => DashboradHelper.IsVisibleForUser(task, user.Id, user.Group?.Name)).ToList();
        
        var boardModel = await DashboradHelper.BuildTaskBoardModelAsync(user, tasks);
        var quickTasks = boardModel.Tasks
            .Where(task => task.Status is not Status.Done and not Status.Resolved)
            .OrderBy(task => task.Deadline)
            .Take(3)
            .ToList();

        LessonDto? nextLesson = null;

        string nextLessonDayLabel = "Today";

        if ( user?.Group != null )
        {
            var schedule = await _scheduleRepository.GetByGroupIdAsync(user.Group.Id);

            if (schedule != null && schedule.Lessons.Any())
            {
                var scheduleDto = new ScheduleDto
                {
                    Id = schedule.Id,
                    UpdateAt = schedule.UpdatedAt,
                    HeadmanUpdate = schedule.CanHeadmanUpdate,
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

                var now = DateTime.Now;
                var currentTime = TimeOnly.FromDateTime(now);
                var currentDayInt = (int)now.DayOfWeek;

                nextLesson = scheduleDto.Lessons
                    .Where(l => (int)l.Day == currentDayInt && l.LessonSlot.StartTime > currentTime)
                    .OrderBy(l => l.LessonSlot.StartTime)
                    .FirstOrDefault();

                if (nextLesson == null)
                {
                    for (int offset = 1; offset <= 6; offset++)
                    {
                        int nextDayInt = (currentDayInt + offset) % 7;

                        if (nextDayInt == 0) continue;

                        nextLesson = scheduleDto.Lessons
                            .Where(l => (int)l.Day == nextDayInt)
                            .OrderBy(l => l.LessonSlot.StartTime)
                            .FirstOrDefault();

                        if ( nextLesson != null )
                        {
                            nextLessonDayLabel = offset == 1 ? "Tomorrow" : nextLesson.Day.ToString();
                            break;
                        }
                    }
                }
            }
        }

        return new DashboardViewModel
        {
            FullName = fullName,
            QuickTasks = quickTasks,
            NextLesson = nextLesson,
            NextLessonDayLabel = nextLessonDayLabel
        };
    }
}