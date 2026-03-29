using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.DTOs;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

public class AdminController(IMediator mediator) : Controller
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