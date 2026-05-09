using MediatR;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Tasks.Commands;

public class UpdateTaskCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Status Status { get; set; }
    public Subject Subject { get; set; }
    public string? ResourceUrl { get; set; }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    public async Task<Guid> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetTaskAsync(request.Id) ?? throw new Exception("Task not found");
        task.Status = request.Status;
        task.Subject = request.Subject;
        task.ResourceUrl = request.ResourceUrl;
        return await _taskRepository.UpdateTaskAsync(task);
    }
}