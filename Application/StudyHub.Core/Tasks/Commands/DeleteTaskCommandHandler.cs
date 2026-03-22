using MediatR;
using StudyHub.Core.Tasks.Interfaces;

namespace StudyHub.Core.Tasks.Commands;

public class DeleteCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    
    public async Task<Guid> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        return await _taskRepository.DeleteTaskAsync(request.Id);
    }
}