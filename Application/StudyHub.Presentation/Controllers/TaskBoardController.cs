using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Queries;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using Application.Models;
using Application.Helpers;

namespace Application.Controllers;

public class TaskBoardController : Controller
{
    private const int TaskTitleMaxLength = 200;
    private const int SummaryMaxLength = 200;
    private const int SubjectDisplayMaxLength = 34;
    private const int CommentMaxLength = 1000;
    private const int CommentUserNameMaxLength = 100;

    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;

    public TaskBoardController(UserManager<User> userManager, IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    [HttpGet("/TaskBoard")]
    public async Task<IActionResult> TaskBoard()
    {
        var model = await BuildTaskBoardModelAsync();
        return View("~/Views/Home/TaskBoard/TaskBoard.cshtml", model);
    }

    [HttpGet("/TaskBoard/Create")]
    [HttpGet("/TaskBoard/CreateTask")]
    public async Task<IActionResult> TaskBoardCreate()
    {
        var model = await BuildTaskCreatePageModelAsync();
        return View("~/Views/Home/TaskBoard/TaskBoardCreate.cshtml", model);
    }

    [HttpPost("/TaskBoard/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardCreate(TaskBoardCreatePageViewModel model)
    {
        model.Title = (model.Title ?? string.Empty).Trim();
        model.Description = TaskFormattingHelper.PrepareSummary(model.Description, SummaryMaxLength);

        if (model.SubjectId == null || model.SubjectId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.SubjectId), "Subject is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(nameof(model.Title), "Task title is required.");
        }

        if (model.Title.Length > TaskTitleMaxLength)
        {
            ModelState.AddModelError(nameof(model.Title), $"Task title cannot exceed {TaskTitleMaxLength} characters.");
        }

        if (model.Description.Length > SummaryMaxLength)
        {
            ModelState.AddModelError(nameof(model.Description), $"Summary cannot exceed {SummaryMaxLength} characters.");
        }

        var maxDueDate = DateTime.Today.AddYears(5);
        if (model.DueDate.Date > maxDueDate)
        {
            ModelState.AddModelError(nameof(model.DueDate), "Due date cannot exceed 5 years from today.");
        }

        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Forbid();
        }

        var selectedSubject = model.SubjectId == null
            ? null
            : await _mediator.Send(new GetSubjectByIdRequest(model.SubjectId.Value));

        if (selectedSubject == null)
        {
            ModelState.AddModelError(nameof(model.SubjectId), "Selected subject was not found.");
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildTaskCreatePageModelAsync(model);
            return View("~/Views/Home/TaskBoard/TaskBoardCreate.cshtml", invalidModel);
        }

        var createdTaskId = await _mediator.Send(new CreateTaskCommand
        {
            Title = model.Title.Length > TaskTitleMaxLength
                ? model.Title[..TaskTitleMaxLength]
                : model.Title,
            Description = model.Description,
            Deadline = model.DueDate,
            IsGroupTask = model.IsGroupTask,
            UserId = currentUserId.Value,
            Subject = new Subject
            {
                Id = selectedSubject!.Id,
                Name = selectedSubject.Name ?? string.Empty
            }
        });

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = createdTaskId });
    }

    [HttpGet("/TaskBoard/ViewTask/{taskCode?}")]
    public async Task<IActionResult> TaskBoardViewTask(string? taskCode)
    {
        var boardModel = await BuildTaskBoardModelAsync();
        TaskBoardTaskCardViewModel? selectedTask = null;
        var comments = new List<Comment>();
        var currentUser = User.Identity?.IsAuthenticated == true
            ? await _userManager.GetUserAsync(User)
            : null;

        ViewBag.CurrentUserFullName = currentUser == null
            ? string.Empty
            : $"{currentUser.Name} {currentUser.Surname}".Trim();

        if (!string.IsNullOrWhiteSpace(taskCode))
        {
            if (Guid.TryParse(taskCode, out var taskId))
            {
                selectedTask = boardModel.Tasks.FirstOrDefault(task => task.Id == taskId);
            }
            else
            {
                selectedTask = boardModel.Tasks.FirstOrDefault(task =>
                    string.Equals(task.TaskCode, taskCode, StringComparison.OrdinalIgnoreCase));
            }
        }

        selectedTask ??= boardModel.Tasks.FirstOrDefault();

        if (selectedTask != null && selectedTask.Id != Guid.Empty)
        {
            comments = await _mediator.Send(new GetCommentsQuery
            {
                TaskId = selectedTask.Id
            });
        }

        var model = new TaskBoardViewTaskPageViewModel
        {
            Board = boardModel,
            SelectedTask = selectedTask,
            Comments = comments
        };

        return View("~/Views/Home/TaskBoard/TaskBoardViewTask.cshtml", model);
    }

    [HttpPost("/TaskBoard/ViewTask/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskDelete(Guid taskId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Forbid();
        }

        var currentUser = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        var taskToDelete = await _mediator.Send(new GetTaskQuery { Id = taskId });
        var taskIsVisible = await _mediator.Send(new IsTaskVisibleForUserQuery
        {
            Task = taskToDelete,
            UserId = currentUserId.Value,
            GroupName = currentUser.GroupName
        });

        if (!taskIsVisible)
        {
            return Forbid();
        }

        await _mediator.Send(new DeleteCommand
        {
            Id = taskId
        });

        return RedirectToAction(nameof(TaskBoard));
    }

    [HttpPost("/TaskBoard/ViewTask/AddComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskAddComment(Guid taskId, string? description)
    {
        var normalizedDescription = TaskFormattingHelper.PrepareSummary(description, CommentMaxLength);
        if (string.IsNullOrWhiteSpace(normalizedDescription))
        {
            return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
        }

        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Forbid();
        }

        var task = await _mediator.Send(new GetTaskQuery { Id = taskId });
        var currentUserDto = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        var taskIsVisible = await _mediator.Send(new IsTaskVisibleForUserQuery
        {
            Task = task,
            UserId = currentUserId.Value,
            GroupName = currentUserDto.GroupName
        });

        if (!taskIsVisible)
        {
            return Forbid();
        }

        var user = User.Identity?.IsAuthenticated == true
            ? await _userManager.GetUserAsync(User)
            : null;

        var userName = user == null
            ? "Guest"
            : $"{user.Name} {user.Surname}".Trim();

        var normalizedUserName = string.IsNullOrWhiteSpace(userName)
            ? "Guest"
            : userName.Trim();

        if (normalizedUserName.Length > CommentUserNameMaxLength)
        {
            normalizedUserName = normalizedUserName[..CommentUserNameMaxLength];
        }

        await _mediator.Send(new CreateCommentCommand
        {
            TaskId = taskId,
            UserName = normalizedUserName,
            Description = normalizedDescription
        });

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpPost("/TaskBoard/ViewTask/DeleteComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskDeleteComment(Guid taskId, Guid commentId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Forbid();
        }

        var task = await _mediator.Send(new GetTaskQuery { Id = taskId });
        var currentUserDto = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        var taskIsVisible = await _mediator.Send(new IsTaskVisibleForUserQuery
        {
            Task = task,
            UserId = currentUserId.Value,
            GroupName = currentUserDto.GroupName
        });

        if (!taskIsVisible)
        {
            return Forbid();
        }

        var comments = await _mediator.Send(new GetCommentsQuery
        {
            TaskId = taskId
        });

        var targetComment = comments.FirstOrDefault(comment => comment.Id == commentId);
        if (targetComment == null)
        {
            return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
        }

        var currentUser = User.Identity?.IsAuthenticated == true
            ? await _userManager.GetUserAsync(User)
            : null;

        var currentUserFullName = currentUser == null
            ? string.Empty
            : $"{currentUser.Name} {currentUser.Surname}".Trim();

        var canDeleteComment = User.IsInRole(nameof(Role.Leader)) ||
                               (!string.IsNullOrWhiteSpace(currentUserFullName) &&
                                string.Equals(targetComment.UserName, currentUserFullName, StringComparison.OrdinalIgnoreCase));

        if (!canDeleteComment)
        {
            return Forbid();
        }

        await _mediator.Send(new DeleteCommentCommand
        {
            Id = commentId
        });

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpPost("/TaskBoard/ViewTask/UpdateStatus")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskUpdateStatus(Guid taskId, string status)
    {
        var parsedStatus = ParseTaskStatus(status);
        var task = await _mediator.Send(new GetTaskQuery { Id = taskId });
        var currentUserId = GetCurrentUserId();

        if (task == null || currentUserId == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return NotFound(new { message = "Task not found." });
            }

            return RedirectToAction(nameof(TaskBoard));
        }

        var currentUser = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        var taskIsVisible = await _mediator.Send(new IsTaskVisibleForUserQuery
        {
            Task = task,
            UserId = currentUserId.Value,
            GroupName = currentUser.GroupName
        });

        if (!taskIsVisible)
        {
            return Forbid();
        }

        await _mediator.Send(new UpdateTaskCommand
        {
            Id = taskId,
            Status = parsedStatus,
            Subject = task.Subject
        });

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Ok(new
            {
                taskId,
                status = status,
                normalizedStatus = NormalizeTaskStatus(parsedStatus)
            });
        }

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpGet("/TaskBoard/ReviewGroup")]
    [Authorize(Roles = nameof(Role.Leader))]
    public async Task<IActionResult> TaskBoardReviewGroup()
    {
        var model = await BuildTaskBoardReviewGroupModelAsync();
        return View("~/Views/Home/TaskBoard/TaskBoardReviewGroup.cshtml", model);
    }

    [HttpPost("/TaskBoard/ReviewGroup/AddUsers")]
    [Authorize(Roles = nameof(Role.Leader))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardReviewGroupAddUsers(List<Guid> selectedUserIds)
    {
        var groupName = await GetCurrentUserGroupNameAsync();
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return RedirectToAction(nameof(TaskBoardReviewGroup));
        }

        foreach (var userId in selectedUserIds.Distinct())
        {
            await _mediator.Send(new AddUserToGroupCommand
            {
                UserId = userId,
                GroupName = groupName
            });
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

    private async Task<TaskBoardPageViewModel> BuildTaskBoardModelAsync()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return new TaskBoardPageViewModel
            {
                Tasks = new List<TaskBoardTaskCardViewModel>(),
                Subjects = new List<string>()
            };
        }

        var currentUser = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        var currentGroupName = currentUser.GroupName;

        var tasks = (await _mediator.Send(new GetVisibleTasksQuery
        {
            UserId = currentUserId.Value,
            GroupName = currentGroupName
        })).ToList();

        var sortedTasks = tasks
            .OrderBy(task => task.Subject?.Name)
            .ThenBy(task => task.CreatedAt)
            .ThenBy(task => task.Title)
            .ToList();

        var subjectCounters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var cards = new List<TaskBoardTaskCardViewModel>(sortedTasks.Count);

        foreach (var task in sortedTasks)
        {
            var subjectName = string.IsNullOrWhiteSpace(task.Subject?.Name) ? "Unknown" : task.Subject.Name;
            if (!subjectCounters.TryAdd(subjectName, 1))
            {
                subjectCounters[subjectName] += 1;
            }

            var prefix = TaskFormattingHelper.GenerateTaskCodePrefix(subjectName);
            var taskCode = $"{prefix}-{subjectCounters[subjectName]}";

            cards.Add(new TaskBoardTaskCardViewModel
            {
                Id = task.Id,
                SubjectId = task.Subject?.Id,
                Title = task.Title,
                Description = task.Description ?? string.Empty,
                SubjectName = subjectName,
                TaskCode = taskCode,
                OwnerName = string.IsNullOrWhiteSpace($"{task.User?.Name} {task.User?.Surname}".Trim())
                    ? "Unknown"
                    : $"{task.User?.Name} {task.User?.Surname}".Trim(),
                OwnerPhotoUrl = ResolveTaskOwnerPhotoUrl(task.User?.PhotoUrl),
                IsGroupTask = task.IsGroupTask,
                Status = task.Status,
                Deadline = task.Deadline
            });
        }

        return new TaskBoardPageViewModel
        {
            Tasks = cards,
            Subjects = cards
                .Select(card => card.SubjectName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(subject => subject)
                .ToList()
        };
    }

    private async Task<TaskBoardCreatePageViewModel> BuildTaskCreatePageModelAsync(TaskBoardCreatePageViewModel? source = null)
    {
        var boardModel = await BuildTaskBoardModelAsync();
        var subjects = await _mediator.Send(new GetAllSubjectsRequest());
        var maxDueDate = DateTime.Today.AddYears(5);

        var dueDate = source?.DueDate ?? DateTime.Today.AddDays(1);
        if (dueDate.Date > maxDueDate)
        {
            dueDate = maxDueDate;
        }

        return new TaskBoardCreatePageViewModel
        {
            Board = boardModel,
            Subjects = subjects
                .OrderBy(subject => subject.Name)
                .Select(subject => new SubjectOptionViewModel
                {
                    Id = subject.Id,
                    Name = subject.Name ?? string.Empty,
                    DisplayName = TaskFormattingHelper.ShortenTextWithEllipsis(subject.Name, SubjectDisplayMaxLength)
                })
                .ToList(),
            Title = source?.Title ?? string.Empty,
            Description = source?.Description ?? string.Empty,
            SubjectId = source?.SubjectId,
            DueDate = dueDate,
            IsGroupTask = source?.IsGroupTask ?? true
        };
    }

    private async Task<TaskBoardReviewGroupPageViewModel> BuildTaskBoardReviewGroupModelAsync()
    {
        var boardModel = await BuildTaskBoardModelAsync();
        var currentGroupName = await GetCurrentUserGroupNameAsync() ?? "No group";
        var users = (await _mediator.Send(new GetUsersRequest())).ToList();

        var groupUsers = users
            .Where(user => string.Equals(user.GroupName, currentGroupName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Surname)
            .Select(user => new GroupUserViewModel
            {
                UserId = user.Id,
                Name = $"{user.Name} {user.Surname}".Trim(),
                Role = (user.Roles?.Any(role => string.Equals(role, nameof(Role.Leader), StringComparison.OrdinalIgnoreCase)) == true)
                    ? "Leader"
                    : "Student"
            })
            .ToList();

        var unassignedUsers = users
            .Where(user => string.IsNullOrWhiteSpace(user.GroupName))
            .Where(user => user.Roles?.Any(role => string.Equals(role, nameof(Role.Admin), StringComparison.OrdinalIgnoreCase)) != true)
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Surname)
            .Select(user => new GroupUserViewModel
            {
                UserId = user.Id,
                Name = $"{user.Name} {user.Surname}".Trim(),
                Role = "Student"
            })
            .ToList();

        return new TaskBoardReviewGroupPageViewModel
        {
            Board = boardModel,
            GroupName = currentGroupName,
            GroupUsers = groupUsers,
            UnassignedUsers = unassignedUsers
        };
    }

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdValue, out var userId))
        {
            return userId;
        }

        return null;
    }

    private async Task<string?> GetCurrentUserGroupNameAsync()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return null;
        }

        var currentUser = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        return currentUser.GroupName;
    }

    private static Status ParseTaskStatus(string rawStatus)
    {
        return rawStatus?.Trim().ToLowerInvariant() switch
        {
            "todo" or "to-do" or "to do" => Status.ToDo,
            "inprogress" or "in-progress" or "in progress" => Status.InProgress,
            "forreview" or "for-review" or "for review" => Status.ForReview,
            "done" => Status.Done,
            "resolved" => Status.Resolved,
            _ => Status.ToDo
        };
    }

    private static string NormalizeTaskStatus(Status status)
    {
        return status switch
        {
            Status.ToDo => "todo",
            Status.InProgress => "in-progress",
            Status.ForReview => "for-review",
            Status.Done => "done",
            Status.Resolved => "done",
            _ => "todo"
        };
    }

    private string ResolveTaskOwnerPhotoUrl(string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return string.Empty;
        }

        const string marker = "user-avatars/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            return Url.Action("UserProfilePhoto", "UserProfile", new { path = photoUrl }) ?? string.Empty;
        }

        return photoUrl;
    }

}
