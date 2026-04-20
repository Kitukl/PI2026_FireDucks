using Application.Helpers;
using Application.Models;
using MediatR;
using StudyHub.Core.Users.Queries;

namespace StudyHub.Core.Tasks.Queries;

public class GetTaskBoardPageQuery : IRequest<TaskBoardPageViewModel>
{
    public Guid? CurrentUserId { get; set; }
}

public class GetTaskBoardPageQueryHandler : IRequestHandler<GetTaskBoardPageQuery, TaskBoardPageViewModel>
{
    private readonly ISender _sender;

    public GetTaskBoardPageQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<TaskBoardPageViewModel> Handle(GetTaskBoardPageQuery request, CancellationToken cancellationToken)
    {
        if (!request.CurrentUserId.HasValue)
        {
            return new TaskBoardPageViewModel
            {
                Tasks = new List<TaskBoardTaskCardViewModel>(),
                Subjects = new List<string>()
            };
        }

        var currentUser = await _sender.Send(new GetUserRequest(request.CurrentUserId.Value), cancellationToken);
        var currentGroupName = currentUser.GroupName;

        var tasks = (await _sender.Send(new GetVisibleTasksQuery
        {
            UserId = request.CurrentUserId.Value,
            GroupName = currentGroupName
        }, cancellationToken)).ToList();

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

    private static string ResolveTaskOwnerPhotoUrl(string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return string.Empty;
        }

        const string marker = "user-avatars/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            var encoded = Uri.EscapeDataString(photoUrl);
            return $"/UserProfile/PhotoFile?path={encoded}";
        }

        return photoUrl;
    }
}
