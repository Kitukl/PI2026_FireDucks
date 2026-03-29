using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Queries;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Storage.DTOs;
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
    private const string UserAvatarContainer = "user-avatars";

    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;
    private readonly IBlobService _blobService;

    public HomeController(
        ILogger<HomeController> logger,
        UserManager<User> userManager,
        IMediator mediator,
        IBlobService blobService)
    {
        _logger = logger;
        _userManager = userManager;
        _mediator = mediator;
        _blobService = blobService;
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
                ViewBag.PhotoUrl = ResolveUserPhotoUrl(user.PhotoUrl);
            }
        }

        ViewBag.FullName ??= "Гість";
        ViewBag.PhotoUrl ??= Url.Content("~/images/no-photo.png");

        return View();
    }

    [HttpPost("/UserProfile/Photo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile? photoFile, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (photoFile is not { Length: > 0 })
        {
            TempData["ProfileFeedbackError"] = "Please select an image first.";
            return RedirectToAction(nameof(UserProfile));
        }

        var safeFileName = Path.GetFileName(photoFile.FileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            safeFileName = $"avatar-{Guid.NewGuid():N}.bin";
        }

        var blobName = $"{user.Id:D}/{Guid.NewGuid():N}-{safeFileName}";
        await using var stream = photoFile.OpenReadStream();

        await _blobService.UploadFileAsync(
            UserAvatarContainer,
            blobName,
            stream,
            photoFile.ContentType,
            cancellationToken);

        if (TryExtractAvatarBlobName(user.PhotoUrl, out var oldBlobName))
        {
            await _blobService.DeleteFileAsync(UserAvatarContainer, oldBlobName, cancellationToken);
        }

        user.PhotoUrl = $"{UserAvatarContainer}/{blobName}";
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            TempData["ProfileFeedbackError"] = "Failed to save profile photo.";
            return RedirectToAction(nameof(UserProfile));
        }

        TempData["ProfileFeedbackSuccess"] = "Profile photo updated.";
        return RedirectToAction(nameof(UserProfile));
    }

    [HttpGet("/UserProfile/PhotoFile")]
    public async Task<IActionResult> UserProfilePhoto([FromQuery] string path, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return NotFound();
        }

        var marker = UserAvatarContainer + "/";
        var blobName = path.StartsWith(marker, StringComparison.OrdinalIgnoreCase)
            ? path[marker.Length..]
            : path;

        var fileStream = await _blobService.GetFileAsync(UserAvatarContainer, blobName, cancellationToken);
        if (fileStream == null)
        {
            return NotFound();
        }

        return File(fileStream, GetImageContentType(blobName));
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
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Forbid();
        }

        var currentUser = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        var taskToDelete = await _mediator.Send(new GetTaskQuery { Id = taskId });
        if (!IsTaskVisibleForUser(taskToDelete, currentUserId.Value, currentUser.GroupName))
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
        if (string.IsNullOrWhiteSpace(description))
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
        if (!IsTaskVisibleForUser(task, currentUserId.Value, currentUserDto.GroupName))
        {
            return Forbid();
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
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Forbid();
        }

        var task = await _mediator.Send(new GetTaskQuery { Id = taskId });
        var currentUserDto = await _mediator.Send(new GetUserRequest(currentUserId.Value));
        if (!IsTaskVisibleForUser(task, currentUserId.Value, currentUserDto.GroupName))
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
        if (!IsTaskVisibleForUser(task, currentUserId.Value, currentUser.GroupName))
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

    [HttpGet("/TaskBoard/ReviewGroup")]
    [Authorize(Roles = nameof(Role.Leader))]
    public async Task<IActionResult> TaskBoardReviewGroup()
    {
        var model = await BuildTaskBoardReviewGroupModelAsync();
        return View(model);
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

    [HttpGet("/Storage")]
    public async Task<IActionResult> Storage(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        var model = await BuildStoragePageModelAsync(user, cancellationToken);
        return View(model);
    }

    [HttpPost("/Storage/Upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadStorageFile(IFormFile? uploadFile, bool shareWithGroup, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (uploadFile is not { Length: > 0 })
        {
            TempData["StorageError"] = "Please choose a file to upload.";
            return RedirectToAction(nameof(Storage));
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        var hasGroup = !string.IsNullOrWhiteSpace(userDto.GroupName);

        var targetContainer = BuildUserStorageContainerName(user.Id);
        var isGroupUpload = false;
        if (shareWithGroup && hasGroup)
        {
            targetContainer = BuildGroupStorageContainerName(userDto.GroupName!);
            isGroupUpload = true;
        }

        var originalName = Path.GetFileName(uploadFile.FileName);
        if (string.IsNullOrWhiteSpace(originalName))
        {
            originalName = $"file-{Guid.NewGuid():N}";
        }

        var blobName = BuildStoredBlobName(originalName);
        await using var stream = uploadFile.OpenReadStream();
        await _blobService.UploadFileAsync(targetContainer, blobName, stream, uploadFile.ContentType, cancellationToken);

        TempData["StorageMessage"] = isGroupUpload
            ? "File uploaded to your group storage."
            : "File uploaded to your personal storage.";

        return RedirectToAction(nameof(Storage));
    }

    [HttpGet("/Storage/Download")]
    public async Task<IActionResult> DownloadStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return NotFound();
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        string containerName;

        if (string.Equals(scope, "group", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(userDto.GroupName))
            {
                return Forbid();
            }

            containerName = BuildGroupStorageContainerName(userDto.GroupName);
        }
        else
        {
            containerName = BuildUserStorageContainerName(user.Id);
        }

        var safeName = Path.GetFileName(name);
        var stream = await _blobService.GetFileAsync(containerName, safeName, cancellationToken);
        if (stream == null)
        {
            return NotFound();
        }

        return File(stream, GetContentTypeByFileName(safeName), StripStoredFilePrefix(safeName));
    }

    [HttpGet("/Storage/Preview")]
    public async Task<IActionResult> PreviewStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return NotFound();
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        string containerName;

        if (string.Equals(scope, "group", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(userDto.GroupName))
            {
                return Forbid();
            }

            containerName = BuildGroupStorageContainerName(userDto.GroupName);
        }
        else
        {
            containerName = BuildUserStorageContainerName(user.Id);
        }

        var safeName = Path.GetFileName(name);
        var stream = await _blobService.GetFileAsync(containerName, safeName, cancellationToken);
        if (stream == null)
        {
            return NotFound();
        }

        return File(stream, GetContentTypeByFileName(safeName));
    }

    [HttpPost("/Storage/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStorageFile(string scope, string name, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Forbid();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["StorageError"] = "File name is required.";
            return RedirectToAction(nameof(Storage));
        }

        var userDto = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);
        string containerName;

        if (string.Equals(scope, "group", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(userDto.GroupName))
            {
                return Forbid();
            }

            containerName = BuildGroupStorageContainerName(userDto.GroupName);
        }
        else
        {
            containerName = BuildUserStorageContainerName(user.Id);
        }

        var safeName = Path.GetFileName(name);
        await _blobService.DeleteFileAsync(containerName, safeName, cancellationToken);

        TempData["StorageMessage"] = "File deleted.";
        return RedirectToAction(nameof(Storage));
    }

    [HttpGet("/Schedule")]
    public IActionResult Schedule()
    {
        return View();
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

        var tasks = (await _mediator.Send(new GetTasksQuery()))
            .Where(task => IsTaskVisibleForUser(task, currentUserId.Value, currentGroupName))
            .ToList();

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

    private static bool IsTaskVisibleForUser(StudyHub.Domain.Entities.Task task, Guid currentUserId, string? currentGroupName)
    {
        if (task.User?.Id == currentUserId)
        {
            return true;
        }

        if (!task.IsGroupTask)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(currentGroupName))
        {
            return false;
        }

        return string.Equals(task.User?.Group?.Name, currentGroupName, StringComparison.OrdinalIgnoreCase);
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

    private async Task<StoragePageViewModel> BuildStoragePageModelAsync(User user, CancellationToken cancellationToken)
    {
        var model = new StoragePageViewModel();
        var currentUser = await _mediator.Send(new GetUserRequest(user.Id), cancellationToken);

        var personalContainer = BuildUserStorageContainerName(user.Id);
        var personalFiles = await _blobService.ListFilesAsync(personalContainer, cancellationToken);
        model.Files.AddRange(personalFiles.Select(file => MapStorageFile(file, "personal", false)));

        if (!string.IsNullOrWhiteSpace(currentUser.GroupName))
        {
            model.GroupName = currentUser.GroupName;
            model.CanShareWithGroup = true;

            var groupContainer = BuildGroupStorageContainerName(currentUser.GroupName);
            var groupFiles = await _blobService.ListFilesAsync(groupContainer, cancellationToken);
            model.Files.AddRange(groupFiles.Select(file => MapStorageFile(file, "group", true)));
        }

        model.Files = model.Files
            .OrderByDescending(item => item.LastModified)
            .ThenBy(item => item.Name)
            .ToList();

        return model;
    }

    private static StorageFileItemViewModel MapStorageFile(BlobFileInfoDto blobFile, string scope, bool isGroupFile)
    {
        var rawName = Path.GetFileName(blobFile.Name);
        var cleanName = StripStoredFilePrefix(rawName);
        var ext = Path.GetExtension(cleanName).TrimStart('.').ToUpperInvariant();

        return new StorageFileItemViewModel
        {
            Name = cleanName,
            BlobName = rawName,
            DownloadName = cleanName,
            Scope = scope,
            Extension = string.IsNullOrWhiteSpace(ext) ? "-" : ext,
            SizeDisplay = FormatFileSize(blobFile.Size),
            LastModified = blobFile.LastModified,
            LastModifiedDisplay = blobFile.LastModified?.ToLocalTime().ToString("dd.MM.yyyy HH:mm") ?? "-",
            CanPreview = CanPreviewFileByExtension(ext),
            IsGroupFile = isGroupFile
        };
    }

    private static string BuildUserStorageContainerName(Guid userId)
    {
        return $"user-storage-{userId:D}";
    }

    private static string BuildGroupStorageContainerName(string groupName)
    {
        return $"group-storage-{groupName}";
    }

    private static string BuildStoredBlobName(string originalName)
    {
        return $"{Guid.NewGuid():N}__{originalName}";
    }

    private static string StripStoredFilePrefix(string fileName)
    {
        var normalized = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return fileName;
        }

        var separatorIndex = normalized.IndexOf("__", StringComparison.Ordinal);
        if (separatorIndex > 0 && separatorIndex + 2 < normalized.Length)
        {
            return normalized[(separatorIndex + 2)..];
        }

        var dashIndex = normalized.IndexOf('-', StringComparison.Ordinal);
        if (dashIndex > 20 && dashIndex + 1 < normalized.Length)
        {
            return normalized[(dashIndex + 1)..];
        }

        return normalized;
    }

    private static bool CanPreviewFileByExtension(string extension)
    {
        return extension switch
        {
            "PNG" => true,
            "JPG" => true,
            "JPEG" => true,
            "WEBP" => true,
            "GIF" => true,
            "BMP" => true,
            "PDF" => true,
            "TXT" => true,
            "CSV" => true,
            _ => false
        };
    }

    private static string GetContentTypeByFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            _ => "application/octet-stream"
        };
    }

    private static string FormatFileSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        double size = bytes;
        var unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:0.#} {units[unitIndex]}";
    }

    private string ResolveUserPhotoUrl(string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return Url.Content("~/images/no-photo.png");
        }

        var marker = UserAvatarContainer + "/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            return Url.Action(nameof(UserProfilePhoto), new { path = photoUrl })
                   ?? Url.Content("~/images/no-photo.png");
        }

        return photoUrl;
    }

    private static bool TryExtractAvatarBlobName(string? photoUrl, out string blobName)
    {
        blobName = string.Empty;
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return false;
        }

        var marker = UserAvatarContainer + "/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            blobName = photoUrl[marker.Length..];
            return !string.IsNullOrWhiteSpace(blobName);
        }

        if (!Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var absolutePath = uri.AbsolutePath.Replace('\\', '/');
        var markerIndex = absolutePath.IndexOf("/" + marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return false;
        }

        var start = markerIndex + marker.Length + 1;
        if (start >= absolutePath.Length)
        {
            return false;
        }

        blobName = absolutePath[start..];
        return !string.IsNullOrWhiteSpace(blobName);
    }

    private static string GetImageContentType(string blobName)
    {
        var extension = Path.GetExtension(blobName);
        return extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}