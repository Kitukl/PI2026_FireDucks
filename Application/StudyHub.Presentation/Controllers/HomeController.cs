using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Queries;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IMediator mediator)
    {
        _logger = logger;
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var fullName = "Гість";

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user != null)
            {
                fullName = $"{user.Name} {user.Surname}".Trim();
            }
        }
        else
        {
            _logger.LogInformation("Anonymous user opened the Index page");
        }

        var boardModel = await BuildTaskBoardModelAsync();
        var quickTasks = boardModel.Tasks
            .Where(task => task.Status is not Status.Done and not Status.Resolved)
            .OrderBy(task => task.Deadline)
            .Take(3)
            .ToList();

        var dashboardModel = new DashboardViewModel
        {
            FullName = fullName,
            QuickTasks = quickTasks
        };

        return View(dashboardModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("/myprofile")]
    [HttpGet("/UserProfile")]
    public async Task<IActionResult> UserProfile()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.FullName = $"{user.Name} {user.Surname}".Trim();
                ViewBag.PhotoUrl = string.IsNullOrWhiteSpace(user.PhotoUrl)
                    ? Url.Content("~/images/no-photo.png")
                    : user.PhotoUrl;
            }
        }

        ViewBag.FullName ??= "Гість";
        ViewBag.PhotoUrl ??= Url.Content("~/images/no-photo.png");

        return View();
    }

    [HttpPost("/UserProfile/SendRequest")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfileSendRequest(
        FeedbackType feedbackType,
        Category category,
        string? subject,
        string? description)
    {
        var user = User.Identity?.IsAuthenticated == true
            ? await _userManager.GetUserAsync(User)
            : null;

        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(description) && string.IsNullOrWhiteSpace(subject))
        {
            TempData["ProfileFeedbackError"] = "Please fill in request details before sending.";
            return RedirectToAction(nameof(UserProfile));
        }

        var fullDescription = string.IsNullOrWhiteSpace(subject)
            ? (description ?? string.Empty).Trim()
            : $"Subject: {subject.Trim()}\n\n{(description ?? string.Empty).Trim()}";

        await _mediator.Send(new CreateFeedbackCommand
        {
            UserId = user.Id,
            FeedbackType = feedbackType,
            Category = category,
            Description = fullDescription,
            Status = Status.ToDo
        });

        TempData["ProfileFeedbackSuccess"] = "Request sent.";
        return RedirectToAction(nameof(UserProfile));
    }

    [HttpGet("/TaskBoard")]
    public async Task<IActionResult> TaskBoard()
    {
        var model = await BuildTaskBoardModelAsync();
        return View(model);
    }

    [HttpGet("/TaskBoard/Create")]
    [HttpGet("/TaskBoard/CreateTask")]
    public async Task<IActionResult> TaskBoardCreate()
    {
        var model = await BuildTaskCreatePageModelAsync();
        return View(model);
    }

    [HttpPost("/TaskBoard/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardCreate(TaskBoardCreatePageViewModel model)
    {
        if (model.SubjectId == null || model.SubjectId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.SubjectId), "Subject is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(nameof(model.Title), "Task title is required.");
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
            return View(invalidModel);
        }

        var createdTaskId = await _mediator.Send(new CreateTaskCommand
        {
            Title = model.Title.Trim(),
            Description = (model.Description ?? string.Empty).Trim(),
            Deadline = model.DueDate,
            IsGroupTask = model.IsGroupTask,
            UserId = currentUserId.Value,
            Subject = new Subject
            {
                Id = selectedSubject!.Id,
                Name = selectedSubject.Name
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

        return View(model);
    }

    [HttpPost("/TaskBoard/ViewTask/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskDelete(Guid taskId)
    {
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
        if (string.IsNullOrWhiteSpace(description))
        {
            return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
        }

        var user = User.Identity?.IsAuthenticated == true
            ? await _userManager.GetUserAsync(User)
            : null;

        var userName = user == null
            ? "Guest"
            : $"{user.Name} {user.Surname}".Trim();

        await _mediator.Send(new CreateCommentCommand
        {
            TaskId = taskId,
            UserName = string.IsNullOrWhiteSpace(userName) ? "Guest" : userName,
            Description = description.Trim()
        });

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpPost("/TaskBoard/ViewTask/DeleteComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardViewTaskDeleteComment(Guid taskId, Guid commentId)
    {
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

        await _mediator.Send(new UpdateTaskCommand
        {
            Id = taskId,
            Status = parsedStatus,
            Subject = task.Subject
        });

        return RedirectToAction(nameof(TaskBoardViewTask), new { taskCode = taskId });
    }

    [HttpGet("/TaskBoard/ReviewGroup")]
    public async Task<IActionResult> TaskBoardReviewGroup()
    {
        var model = await BuildTaskBoardReviewGroupModelAsync();
        return View(model);
    }

    [HttpPost("/TaskBoard/ReviewGroup/AddUsers")]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaskBoardReviewGroupRemoveUser(Guid userId)
    {
        await _mediator.Send(new RemoveUserFromGroupCommand
        {
            UserId = userId
        });

        return RedirectToAction(nameof(TaskBoardReviewGroup));
    }

    [HttpGet("/Storage")]
    public IActionResult Storage()
    {
        return View();
    }

    private async Task<TaskBoardPageViewModel> BuildTaskBoardModelAsync()
    {
        var tasks = (await _mediator.Send(new GetTasksQuery())).ToList();

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

            var prefix = char.ToUpperInvariant(subjectName[0]);
            var taskCode = $"{prefix}-{subjectCounters[subjectName]}";

            cards.Add(new TaskBoardTaskCardViewModel
            {
                Id = task.Id,
                SubjectId = task.Subject?.Id,
                Title = task.Title,
                Description = task.Description ?? string.Empty,
                SubjectName = subjectName,
                TaskCode = taskCode,
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

        return new TaskBoardCreatePageViewModel
        {
            Board = boardModel,
            Subjects = subjects
                .OrderBy(subject => subject.Name)
                .Select(subject => new SubjectOptionViewModel
                {
                    Id = subject.Id,
                    Name = subject.Name
                })
                .ToList(),
            Title = source?.Title ?? string.Empty,
            Description = source?.Description ?? string.Empty,
            SubjectId = source?.SubjectId,
            DueDate = source?.DueDate ?? DateTime.Today.AddDays(1),
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}