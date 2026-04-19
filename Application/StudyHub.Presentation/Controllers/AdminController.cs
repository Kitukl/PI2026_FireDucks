using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

[Authorize(Roles = nameof(Role.Admin))]
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
            StudentsCount = user.StudentsCount,
            GroupsCount = user.GroupsCount,
            LeadersCount = user.LeadersCount,
            UserFilesCount = user.UserFilesCount,
            GroupFilesCount = user.GroupFilesCount,
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
    public IActionResult GetUser(Guid id)
    {
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserModal(Guid id)
    {
        var viewModel = await BuildUpdateUserViewModel(id);
        return PartialView("_UpdateUserForm", viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
        var normalizedSelectedRoles = (model.SelectedRoles ?? [])
            .Where(role => Enum.GetNames(typeof(Role)).Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        model.SelectedRoles = normalizedSelectedRoles;

        if (normalizedSelectedRoles.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Changes cannot be saved. Please add at least one role.");
        }

        var hasAdminRole = normalizedSelectedRoles.Any(role => string.Equals(role, nameof(Role.Admin), StringComparison.OrdinalIgnoreCase));
        var hasStudentRole = normalizedSelectedRoles.Any(role => string.Equals(role, nameof(Role.Student), StringComparison.OrdinalIgnoreCase));
        var hasLeaderRole = normalizedSelectedRoles.Any(role => string.Equals(role, nameof(Role.Leader), StringComparison.OrdinalIgnoreCase));

        if (hasLeaderRole && !hasStudentRole)
        {
            ModelState.AddModelError(string.Empty, "Changes cannot be saved. Leader should has  Student role.");
        }

        if (hasAdminRole && (hasStudentRole || hasLeaderRole))
        {
            ModelState.AddModelError(string.Empty, "Changes cannot be saved. Admin role cannot be provided to Student.");
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildUpdateUserViewModel(model.Id, model);
            if (IsAjaxRequest())
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return PartialView("_UpdateUserForm", invalidModel);
            }

            return View("UpdateUser", invalidModel);
        }

        var userDto = await mediator.Send(new GetUserRequest(model.Id));
        var existingRoles = (userDto.Roles ?? [])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var rolesToRemove = existingRoles
            .Where(role => !normalizedSelectedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();
        var rolesToAdd = normalizedSelectedRoles
            .Where(role => !existingRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var role in rolesToRemove)
        {
            if (!Enum.TryParse<Role>(role, true, out var parsedRole))
            {
                continue;
            }

            await mediator.Send(new RemoveUserRoleCommand
            {
                UserId = model.Id,
                Role = parsedRole
            });
        }

        foreach (var role in rolesToAdd)
        {
            if (!Enum.TryParse<Role>(role, true, out var parsedRole))
            {
                continue;
            }

            await mediator.Send(new AssignUserRoleCommand
            {
                UserId = model.Id,
                Role = parsedRole
            });
        }

        await mediator.Send(new UpdateUserCommand
        {
            Id = model.Id,
            GroupName = model.GroupName ?? string.Empty
        });

        if (IsAjaxRequest())
        {
            return Json(new
            {
                success = true,
                message = "User changes were saved.",
                userId = model.Id,
                roles = normalizedSelectedRoles,
                groupName = model.GroupName ?? string.Empty
            });
        }

        TempData["AdminUsersMessage"] = "User changes were saved.";
        return RedirectToAction(nameof(Users));
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await mediator.Send(new DeleteUserCommand { UserId = id });
    
        return RedirectToAction("Users"); 
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
            AutoUpdateIntervalDays = firstWithSettings?.UpdateInterval ?? 3
        };

        if (groupId.HasValue)
        {
            var scheduleDto = await mediator.Send(new GetScheduleByGroupIdRequest(groupId.Value));
            if (scheduleDto != null)
            {
                model.SelectedGroupLastUpdate = scheduleDto.UpdateAt;
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
    public async Task<IActionResult> UpdateGlobalSettings(bool isAutoUpdate, bool allowLeaders, uint intervalDays)
    {
        Console.WriteLine($"DEBUG: AutoUpdate={isAutoUpdate}, AllowLeaders={allowLeaders}, Interval={intervalDays}");

        await _context.Schedules.ExecuteUpdateAsync(s => s
            .SetProperty(b => b.IsAutoUpdate, isAutoUpdate)
            .SetProperty(b => b.CanHeadmanUpdate, allowLeaders)
            .SetProperty(b => b.UpdateInterval, intervalDays)
            .SetProperty(b => b.UpdatedAt, DateTime.UtcNow));

        return RedirectToAction(nameof(Schedule));
    }

    private ScheduleViewModel BuildScheduleGridViewModel(ScheduleDto dto)
    {
        var vm = new ScheduleViewModel
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

    private async Task<UpdateUserViewModel> BuildUpdateUserViewModel(Guid id, UpdateUserViewModel? source = null)
    {
        var userDto = await mediator.Send(new GetUserRequest(id));
        var users = await mediator.Send(new GetUsersRequest());
        var existingGroups = users
            .Select(user => user.GroupName)
            .Where(groupName => !string.IsNullOrWhiteSpace(groupName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(groupName => groupName)
            .ToList();

        var availableRoles = Enum.GetNames(typeof(Role)).ToList();
        var selectedRoles = source?.SelectedRoles?.Any() == true
            ? source.SelectedRoles
            : (userDto.Roles ?? []);

        return new UpdateUserViewModel
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Surname = userDto.Surname ?? string.Empty,
            PhotoUrl = userDto.PhotoUrl ?? string.Empty,
            GroupName = source?.GroupName ?? userDto.GroupName ?? string.Empty,
            Roles = userDto.Roles ?? [],
            SelectedRoles = selectedRoles
                .Where(role => availableRoles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(),
            AvailableRoles = availableRoles,
            ExistingGroups = existingGroups
        };
    }

    private bool IsAjaxRequest()
    {
        if (!Request.Headers.TryGetValue("X-Requested-With", out var headerValue))
        {
            return false;
        }

        return string.Equals(headerValue.ToString(), "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}