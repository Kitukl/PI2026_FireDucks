using MediatR;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Tasks.Commands;

public class UpdateTaskFullCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ResourceUrl { get; set; }
    public Status Status { get; set; }
    public Subject Subject { get; set; } = null!;

}

public class UpdateTaskFullCommandHandler : IRequestHandler<UpdateTaskFullCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskFullCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Guid> Handle(UpdateTaskFullCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetTaskAsync(request.Id) ?? throw new Exception("Task not found");

        task.Title = request.Title;
        task.Description = request.Description;
        task.ResourceUrl = request.ResourceUrl;
        task.Status = request.Status;
        task.Subject = request.Subject;

        return await _taskRepository.UpdateTaskAsync(task);
    }
}