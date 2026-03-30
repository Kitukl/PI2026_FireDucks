using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Queries;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using StudyHub.Infrastructure;
using DayOfWeek = StudyHub.Domain.Enums.DayOfWeek;

namespace Application.Controllers;

public class AdminController(IMediator mediator, SDbContext _context) : Controller
{
    public async Task<IActionResult> Dashboard()
    {
        var user = await mediator.Send(new GetUsersStatisticRequest());
        var tasksCount = await mediator.Send(new GetTaskCountRequest());
        var taskStatusCount = await mediator.Send(new GetGroupedTaskStatsRequest());
        
        var viewModel = new SystemStatisticViewModel()
        {
            CreatedAt = user.CreatedAt,
            UserActivityPerMonth = user.UserActivityPerMonth,
            GropedTaskCount = taskStatusCount,
            FileCount = user.FileCount,
            TaskCount = tasksCount
        };
            
        return View(viewModel);
    }

    public async Task<IActionResult> Users()
    {
        var users = await mediator.Send(new GetUsersRequest());
        
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendGlobalAnnouncement(string subject, string description)
    {
        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(description))
        {
            TempData["AdminUsersError"] = "Subject and description are required.";
            return RedirectToAction(nameof(Users));
        }

        try
        {
            var recipientsCount = await mediator.Send(new SendGlobalAnnouncementCommand
            {
                Subject = subject,
                Description = description
            });

            TempData["AdminUsersMessage"] = recipientsCount > 0
                ? $"Global announcement sent to {recipientsCount} users."
                : "No recipients were found for global announcement.";
        }
        catch (Exception ex)
        {
            TempData["AdminUsersError"] = $"Unable to send global announcement: {ex.Message}";
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var userDto = await mediator.Send(new GetUserRequest(id));
        var users = await mediator.Send(new GetUsersRequest());
        var existingGroups = users
            .Select(user => user.GroupName)
            .Where(groupName => !string.IsNullOrWhiteSpace(groupName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(groupName => groupName)
            .ToList();
        
        var viewModel = new UpdateUserViewModel
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Surname = userDto.Surname ?? "",
            PhotoUrl = userDto.PhotoUrl ?? string.Empty,
            GroupName = userDto.GroupName,
            Roles = userDto.Roles,
            AvailableRoles = Enum.GetNames(typeof(Role)).ToList(),
            ExistingGroups = existingGroups
        };
        
        return View("UpdateUser",viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateUser(Guid id, string groupName)
    {
        await mediator.Send(new UpdateUserCommand 
        { 
            Id = id,
            GroupName = groupName
        });
    
        return RedirectToAction("GetUser", new { id });
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await mediator.Send(new DeleteUserCommand { UserId = id });
    
        return RedirectToAction("Users"); 
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUserRole(UserRoleUpdateDto dto)
    {
        await mediator.Send(new AssignUserRoleCommand 
        { 
            UserId = dto.Id, 
            Role = dto.Role 
        });
    
        return RedirectToAction("GetUser", new { id = dto.Id });
    }

    [HttpPost] 
    public async Task<IActionResult> RemoveUserRole(UserRoleUpdateDto dto)
    {
        await mediator.Send(new RemoveUserRoleCommand 
        { 
            UserId = dto.Id, 
            Role = dto.Role 
        });
    
        return RedirectToAction("GetUser", new { id = dto.Id });
    }

    [HttpGet("/Admin/Requests")]
    public async Task<IActionResult> Requests()
    {
        var feedbacks = await mediator.Send(new GetFeedbacksCommand());
        var model = BuildRequestsViewModel(feedbacks, null, false);
        return View(model);
    }

    [HttpGet("/Admin/Requests/View/{feedbackId?}")]
    public async Task<IActionResult> RequestView(string? feedbackId)
    {
        var feedbacks = await mediator.Send(new GetFeedbacksCommand());

        Feedback? activeRequest = null;
        if (!string.IsNullOrWhiteSpace(feedbackId) && Guid.TryParse(feedbackId, out var parsedId))
        {
            activeRequest = feedbacks.FirstOrDefault(item => item.Id == parsedId);
            if (activeRequest == null)
            {
                try
                {
                    activeRequest = await mediator.Send(new GetFeedbackCommand { Id = parsedId });
                }
                catch
                {
                    activeRequest = null;
                }
            }
        }

        var model = BuildRequestsViewModel(feedbacks, activeRequest, true);
        return View("Requests", model);
    }

    [HttpPost("/Admin/Requests/UpdateStatus")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRequestStatus(Guid feedbackId, Status status)
    {
        var allowedStatuses = new[] { Status.ToDo, Status.InProgress, Status.Resolved };
        if (!allowedStatuses.Contains(status))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return BadRequest(new { message = "Unsupported status." });
            }

            return RedirectToAction(nameof(RequestView), new { feedbackId });
        }

        await mediator.Send(new UpdateFeedbackCommand
        {
            Id = feedbackId,
            Status = status
        });

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Ok(new
            {
                feedbackId,
                status = status.ToString(),
                statusLabel = status switch
                {
                    Status.ToDo => "To do",
                    Status.InProgress => "In progress",
                    Status.Resolved => "Resolved",
                    _ => status.ToString()
                }
            });
        }

        return RedirectToAction(nameof(RequestView), new { feedbackId });
    }

    [HttpGet("/Admin/Schedule/{groupId?}")]
    public async Task<IActionResult> Schedule(Guid? groupId)
    {
        var groups = await _context.Groups
            .OrderBy(g => g.Name)
            .Select(g => new GroupDto { Id = g.Id, Name = g.Name })
            .ToListAsync();

        var allSchedules = await mediator.Send(new GetAllSchedulesRequest());
        var firstWithSettings = allSchedules.FirstOrDefault();

        var model = new AdminScheduleViewModel
        {
            Groups = groups,
            SelectedGroupId = groupId,
            IsAutoUpdateEnabled = firstWithSettings?.IsAutoUpdate ?? false,
            AllowLeadersToUpdate = firstWithSettings?.HeadmanUpdate ?? false,
            LastGlobalUpdate = allSchedules.Any()
                ? allSchedules.Max(s => s.UpdateAt)
                : DateTime.MinValue,
            AutoUpdateIntervalDays = 3
        };

        if (groupId.HasValue)
        {
            var scheduleDto = await mediator.Send(new GetScheduleByGroupIdRequest(groupId.Value));
            if (scheduleDto != null)
            {
                model.CurrentGroupSchedule = BuildScheduleGridViewModel(scheduleDto);
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RunGlobalUpdate()
    {
        var groupNames = await _context.Groups.Select(g => g.Name).ToListAsync();

        foreach (var name in groupNames)
        {
            await mediator.Send(new ParseAndSaveScheduleCommand(name));
        }

        return RedirectToAction(nameof(Schedule));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGroupSchedule(Guid groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group != null)
        {
            await mediator.Send(new ParseAndSaveScheduleCommand(group.Name));
        }
        return RedirectToAction(nameof(Schedule), new { groupId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveGroupSchedule(Guid groupId)
    {
        await mediator.Send(new DeleteScheduleForGroupRequest(groupId));
        return RedirectToAction(nameof(Schedule));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveGlobalSchedule()
    {
        await mediator.Send(new DeleteAllRequest());
        return RedirectToAction(nameof(Schedule));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGlobalSettings(bool isAutoUpdate, bool allowLeaders)
    {
        await mediator.Send(new SetScheduleAutoUpdateRequest(isAutoUpdate));

        var schedules = await _context.Schedules.ToListAsync();
        foreach (var s in schedules)
        {
            s.CanHeadmanUpdate = allowLeaders;
        }
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Schedule));
    }

    private Application.Models.ScheduleViewModel BuildScheduleGridViewModel(ScheduleDto dto)
    {
        var vm = new Application.Models.ScheduleViewModel
        {
            GroupId = dto.Group.Id,
            GroupName = dto.Group.Name,
            CanHeadmanUpdate = dto.HeadmanUpdate,
            IsHeadman = false
        };

        if (dto.Lessons != null && dto.Lessons.Any())
        {
            vm.UniqueSlots = dto.Lessons
                .Select(l => l.LessonSlot)
                .Where(s => s != null)
                .GroupBy(s => s.Id)
                .Select(g => g.First())
                .OrderBy(s => s.StartTime)
                .ToList();

            vm.Days = new List<DayOfWeek>
            {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday
            };

            foreach (var lesson in dto.Lessons)
            {
                var key = $"{(int)lesson.Day}-{lesson.LessonSlot.Id}";

                if (!vm.Grid.ContainsKey(key))
                {
                    vm.Grid[key] = new List<LessonDto>();
                }
                vm.Grid[key].Add(lesson);
            }
        }

        return vm;
    }



    private static AdminRequestsViewModel BuildRequestsViewModel(
        IEnumerable<Feedback> feedbacks,
        Feedback? activeRequest,
        bool openModal)
    {
        var allowedStatuses = new[] { Status.ToDo, Status.InProgress, Status.Resolved };

        var filteredRequests = feedbacks
            .Where(request => allowedStatuses.Contains(request.Status))
            .OrderByDescending(request => request.CreatedAt)
            .ToList();

        if (activeRequest == null)
        {
            activeRequest = filteredRequests.FirstOrDefault();
        }

        return new AdminRequestsViewModel
        {
            Requests = filteredRequests,
            ActiveRequest = activeRequest,
            OpenRequestModal = openModal && activeRequest != null
        };
    }
}