using MediatR;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Tasks.Commands;

public class CreateTaskCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsGroupTask { get; set; }
    public DateTime Deadline { get; set; }
    public Subject Subject { get; set; } = null!;
    public Guid UserId { get; set; }
}

public class CreateTaskCommandsHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public CreateTaskCommandsHandler(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }
    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);
        var normalizedDeadline = request.Deadline.Kind switch
        {
            DateTimeKind.Utc => request.Deadline,
            DateTimeKind.Local => request.Deadline.ToUniversalTime(),
            _ => DateTime.SpecifyKind(request.Deadline, DateTimeKind.Utc)
        };

        var task = new Task
        {
            CreatedAt = DateTime.UtcNow,
            Deadline = normalizedDeadline,
            IsGroupTask = request.IsGroupTask,
            Status = Status.ToDo,
            Subject = request.Subject,
            Title = request.Title,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? string.Empty
                : request.Description.Trim(),
            User = user
        };

        return await _taskRepository.AddTaskAsync(task);
    }
}