using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using System.Security.Claims;

namespace Application.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;

    public DashboardController(
        ILogger<DashboardController> logger,
        UserManager<User> userManager,
        IMediator mediator)
    {
        _logger = logger;
        _userManager = userManager;
        _mediator = mediator;
    }

    [HttpGet("/")]
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

        return View("~/Views/Home/Dashboard/Index.cshtml", dashboardModel);
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

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdValue, out var userId))
        {
            return userId;
        }

        return null;
    }
}
