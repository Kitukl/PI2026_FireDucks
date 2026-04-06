using MediatR;
using StudyHub.Core.Tasks.Interfaces;
using EntityTask = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Tasks.Queries;

public class GetVisibleTasksQuery : IRequest<IEnumerable<EntityTask>>
{
    public Guid UserId { get; set; }
    public string? GroupName { get; set; }
}

public class GetVisibleTasksQueryHandler : IRequestHandler<GetVisibleTasksQuery, IEnumerable<EntityTask>>
{
    private readonly ITaskRepository _taskRepository;

    public GetVisibleTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<EntityTask>> Handle(GetVisibleTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetTasksAsync();

        return tasks.Where(task => IsVisibleForUser(task, request.UserId, request.GroupName));
    }

    private static bool IsVisibleForUser(EntityTask task, Guid userId, string? groupName)
    {
        if (task.User?.Id == userId)
        {
            return true;
        }

        if (!task.IsGroupTask || string.IsNullOrWhiteSpace(groupName))
        {
            return false;
        }

        return string.Equals(task.User?.Group?.Name, groupName, StringComparison.OrdinalIgnoreCase);
    }
}

public class IsTaskVisibleForUserQuery : IRequest<bool>
{
    public required EntityTask Task { get; set; }
    public Guid UserId { get; set; }
    public string? GroupName { get; set; }
}

public class IsTaskVisibleForUserQueryHandler : IRequestHandler<IsTaskVisibleForUserQuery, bool>
{
    public Task<bool> Handle(IsTaskVisibleForUserQuery request, CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.Task.FromResult(IsVisibleForUser(request.Task, request.UserId, request.GroupName));
    }

    private static bool IsVisibleForUser(EntityTask task, Guid userId, string? groupName)
    {
        if (task.User?.Id == userId)
        {
            return true;
        }

        if (!task.IsGroupTask || string.IsNullOrWhiteSpace(groupName))
        {
            return false;
        }

        return string.Equals(task.User?.Group?.Name, groupName, StringComparison.OrdinalIgnoreCase);
    }
}
