using MediatR;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Tasks.Commands;

public class CreateTaskCommand : IRequest<Guid>
{
    public string Title { get; set; }
    public bool IsGroupTask { get; set; }
    public DateTime Deadline { get; set; }
    public Subject Subject { get; set; }
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
        var task = new Task
        {
            CreatedAt = DateTime.UtcNow,
            Deadline = request.Deadline,
            IsGroupTask = request.IsGroupTask,
            Status = Status.ToDo,
            Subject = request.Subject,
            Title = request.Title,
            User = user
        };

        return await _taskRepository.AddTaskAsync(task);
    }
}