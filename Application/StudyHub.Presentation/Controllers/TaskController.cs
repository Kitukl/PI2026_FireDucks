using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.DTOs;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Commands;
using StudyHub.Domain.Enums;
using Application.Models;
using Application.Helpers;

namespace Application.Controllers;

public class TaskController : Controller
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/TaskBoard")]
    public async Task<IActionResult> TaskBoard()
    {
        var model = await _mediator.Send(new GetTaskBoardPageQuery
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User)
        });

        return View("~/Views/Home/TaskBoard/TaskBoard.cshtml", model);
    }

    [HttpGet("/TaskBoard/Create")]
    [HttpGet("/TaskBoard/CreateTask")]
    public async Task<IActionResult> TaskBoardCreate()
    {
        var modelData = await _mediator.Send(new GetTaskBoardCreatePageQuery
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User)
        });

        var model = TaskBoardViewModelMapper.MapTaskCreatePageViewModel(modelData);
        return View("~/Views/Home/TaskBoard/TaskBoardCreate.cshtml", model);
    }

    [HttpPost("/TaskBoard/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardCreate(TaskBoardCreatePageViewModel model)
    {
        var currentUserId = TaskBoardControllerHelper.GetCurrentUserId(User);

        var createResult = await _mediator.Send(new CreateTaskBoardTaskCommand
        {
            CurrentUserId = currentUserId,
            Title = model.Title,
            Description = model.Description,
            SubjectId = model.SubjectId,
            DueDate = model.DueDate,
            IsGroupTask = model.IsGroupTask
        });

        if (createResult.IsForbidden)
        {
            return Forbid();
        }

        if (!createResult.IsSuccess)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(error.Field, error.Message);
            }

            var invalidModelData = createResult.InvalidPageData ?? new TaskBoardCreatePageDataDto
            {
                Title = createResult.Title,
                Description = createResult.Description,
                SubjectId = createResult.SubjectId,
                DueDate = createResult.DueDate,
                IsGroupTask = createResult.IsGroupTask
            };

            var invalidModel = TaskBoardViewModelMapper.MapTaskCreatePageViewModel(invalidModelData);
            return View("~/Views/Home/TaskBoard/TaskBoardCreate.cshtml", invalidModel);
        }

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = createResult.CreatedTaskId });
    }

    [HttpGet("/TaskBoard/ViewTask/{taskCode?}")]
    public async Task<IActionResult> TaskBoardViewTask(string? taskCode)
    {
        var pageData = await _mediator.Send(new GetTaskBoardViewTaskPageQuery
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User),
            TaskCode = taskCode
        });

        ViewBag.CurrentUserFullName = pageData.CurrentUserFullName;

        var model = new TaskBoardViewTaskPageViewModel
        {
            Board = pageData.Board,
            SelectedTask = pageData.SelectedTask,
            Comments = pageData.Comments
        };

        return View("~/Views/Home/TaskBoard/TaskBoardViewTask.cshtml", model);
    }

    [HttpPost("/TaskBoard/ViewTask/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskDelete(Guid taskId)
    {
        var result = await _mediator.Send(new DeleteTaskBoardTaskCommand
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User),
            TaskId = taskId
        });

        if (result.IsForbidden)
        {
            return Forbid();
        }

        return RedirectToAction(nameof(TaskBoard));
    }

    [HttpPost("/TaskBoard/ViewTask/AddComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskAddComment(Guid taskId, string? description)
    {
        var result = await _mediator.Send(new AddTaskBoardCommentCommand
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User),
            TaskId = taskId,
            Description = description
        });

        if (result.IsForbidden)
        {
            return Forbid();
        }

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpPost("/TaskBoard/ViewTask/DeleteComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskDeleteComment(Guid taskId, Guid commentId)
    {
        var result = await _mediator.Send(new DeleteTaskBoardCommentCommand
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User),
            TaskId = taskId,
            CommentId = commentId,
            IsLeader = User.IsInRole(nameof(Role.Leader))
        });

        if (result.IsForbidden)
        {
            return Forbid();
        }

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpPost("/TaskBoard/ViewTask/UpdateStatus")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskUpdateStatus(Guid taskId, string status)
    {
        var result = await _mediator.Send(new UpdateTaskBoardTaskStatusCommand
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User),
            TaskId = taskId,
            Status = status
        });

        if (result.IsNotFound)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return NotFound(new { message = "Task not found." });
            }

            return RedirectToAction(nameof(TaskBoard));
        }

        if (result.IsForbidden)
        {
            return Forbid();
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Ok(new
            {
                taskId,
                status = result.Status,
                normalizedStatus = result.NormalizedStatus
            });
        }

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpGet("/TaskBoard/ReviewGroup")]
    [Authorize(Roles = nameof(Role.Leader))]
    public async Task<IActionResult> TaskBoardReviewGroup()
    {
        var data = await _mediator.Send(new GetTaskBoardReviewGroupPageQuery
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User)
        });

        var model = new TaskBoardReviewGroupPageViewModel
        {
            Board = data.Board,
            GroupName = data.GroupName,
            GroupUsers = data.GroupUsers.Select(TaskBoardViewModelMapper.MapGroupUserViewModel).ToList(),
            UnassignedUsers = data.UnassignedUsers.Select(TaskBoardViewModelMapper.MapGroupUserViewModel).ToList()
        };

        return View("~/Views/Home/TaskBoard/TaskBoardReviewGroup.cshtml", model);
    }

    [HttpPost("/TaskBoard/ReviewGroup/AddUsers")]
    [Authorize(Roles = nameof(Role.Leader))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardReviewGroupAddUsers(List<Guid> selectedUserIds)
    {
        var result = await _mediator.Send(new AddUsersToTaskBoardGroupCommand
        {
            CurrentUserId = TaskBoardControllerHelper.GetCurrentUserId(User),
            SelectedUserIds = selectedUserIds
        });

        if (result.IsForbidden)
        {
            return Forbid();
        }

        return RedirectToAction(nameof(TaskBoardReviewGroup));
    }

    [HttpPost("/TaskBoard/ReviewGroup/RemoveUser")]
    [Authorize(Roles = nameof(Role.Leader))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardReviewGroupRemoveUser(Guid userId)
    {
        await _mediator.Send(new RemoveUserFromGroupCommand
        {
            UserId = userId
        });

        return RedirectToAction(nameof(TaskBoardReviewGroup));
    }

}
