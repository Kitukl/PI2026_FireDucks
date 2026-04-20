using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Admin.Commands;
using StudyHub.Core.Admin.Queries;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

[Authorize(Roles = nameof(Role.Admin))]
public class AdminController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Dashboard()
    {
        var data = await mediator.Send(new GetAdminDashboardQuery());

        var viewModel = new SystemStatisticViewModel
        {
            CreatedAt = data.CreatedAt,
            UserActivityPerMonth = data.UserActivityPerMonth,
            GropedTaskCount = data.GroupedTaskCount,
            StudentsCount = data.StudentsCount,
            GroupsCount = data.GroupsCount,
            LeadersCount = data.LeadersCount,
            UserFilesCount = data.UserFilesCount,
            GroupFilesCount = data.GroupFilesCount,
            FileCount = data.FileCount,
            TaskCount = data.TaskCount
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
        var data = await mediator.Send(new GetAdminUpdateUserFormQuery
        {
            UserId = id
        });

        var viewModel = new UpdateUserViewModel
        {
            Id = data.Id,
            Name = data.Name,
            Surname = data.Surname,
            PhotoUrl = data.PhotoUrl,
            GroupName = data.GroupName,
            Roles = data.Roles,
            SelectedRoles = data.SelectedRoles,
            AvailableRoles = data.AvailableRoles,
            ExistingGroups = data.ExistingGroups
        };

        return PartialView("_UpdateUserForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
        var result = await mediator.Send(new UpdateAdminUserWithFormCommand
        {
            UserId = model.Id,
            GroupName = model.GroupName,
            SelectedRoles = model.SelectedRoles
        });

        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

        if (result.HasValidationErrors)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var invalidData = result.InvalidData;
            var invalidModel = invalidData == null
                ? new UpdateUserViewModel { Id = model.Id }
                : new UpdateUserViewModel
                {
                    Id = invalidData.Id,
                    Name = invalidData.Name,
                    Surname = invalidData.Surname,
                    PhotoUrl = invalidData.PhotoUrl,
                    GroupName = invalidData.GroupName,
                    Roles = invalidData.Roles,
                    SelectedRoles = invalidData.SelectedRoles,
                    AvailableRoles = invalidData.AvailableRoles,
                    ExistingGroups = invalidData.ExistingGroups
                };

            if (isAjaxRequest)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return PartialView("_UpdateUserForm", invalidModel);
            }

            return View("UpdateUser", invalidModel);
        }

        if (isAjaxRequest)
        {
            return Json(new
            {
                success = true,
                message = "User changes were saved.",
                userId = result.UserId,
                roles = result.Roles,
                groupName = result.GroupName
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
        var data = await mediator.Send(new GetAdminRequestsPageQuery
        {
            OpenModal = false
        });

        var model = new AdminRequestsViewModel
        {
            Requests = data.Requests,
            ActiveRequest = data.ActiveRequest,
            OpenRequestModal = data.OpenRequestModal
        };

        return View(model);
    }

    [HttpGet("/Admin/Requests/View/{feedbackId?}")]
    public async Task<IActionResult> RequestView(string? feedbackId)
    {
        var data = await mediator.Send(new GetAdminRequestsPageQuery
        {
            FeedbackId = feedbackId,
            OpenModal = true
        });

        var model = new AdminRequestsViewModel
        {
            Requests = data.Requests,
            ActiveRequest = data.ActiveRequest,
            OpenRequestModal = data.OpenRequestModal
        };

        return View("Requests", model);
    }

    [HttpPost("/Admin/Requests/UpdateStatus")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRequestStatus(Guid feedbackId, Status status)
    {
        var result = await mediator.Send(new UpdateAdminRequestStatusCommand
        {
            FeedbackId = feedbackId,
            Status = status
        });

        var isAjaxRequest = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

        if (!result.IsSuccess)
        {
            if (isAjaxRequest)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return RedirectToAction(nameof(RequestView), new { feedbackId });
        }

        if (isAjaxRequest)
        {
            return Ok(new
            {
                feedbackId,
                status = result.Status,
                statusLabel = result.StatusLabel
            });
        }

        return RedirectToAction(nameof(RequestView), new { feedbackId });
    }

    [HttpGet("/Admin/Schedule/{groupId?}")]
    public async Task<IActionResult> Schedule(Guid? groupId)
    {
        var data = await mediator.Send(new GetAdminSchedulePageQuery
        {
            GroupId = groupId
        });

        var model = new AdminScheduleViewModel
        {
            Groups = data.Groups,
            SelectedGroupId = data.SelectedGroupId,
            IsAutoUpdateEnabled = data.IsAutoUpdateEnabled,
            AllowLeadersToUpdate = data.AllowLeadersToUpdate,
            LastGlobalUpdate = data.LastGlobalUpdate,
            AutoUpdateIntervalDays = data.AutoUpdateIntervalDays,
            SelectedGroupLastUpdate = data.SelectedGroupLastUpdate,
            CurrentGroupSchedule = data.CurrentGroupSchedule ?? new ScheduleViewModel()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RunGlobalUpdate()
    {
        await mediator.Send(new RunGlobalScheduleUpdateRequest());

        return RedirectToAction(nameof(Schedule));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGroupSchedule(Guid groupId)
    {
        await mediator.Send(new UpdateGroupScheduleRequest(groupId));

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
        await mediator.Send(new UpdateGlobalScheduleSettingsRequest(isAutoUpdate, allowLeaders, intervalDays));

        return RedirectToAction(nameof(Schedule));
    }
}