using MediatR;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Comments.Commands;

public class CreateCommentCommand : IRequest<Guid>
{
    public string UserName { get; set; }
    public string Description { get; set; }
    public Guid TaskId { get; set; }
}

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICommentRepository _repository;
    private readonly ITaskRepository _taskRepository;

    public CreateCommentCommandHandler(ICommentRepository repository, ITaskRepository taskRepository)
    {
        _repository = repository;
        _taskRepository = taskRepository;
    }
    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetTaskAsync(request.TaskId) ?? throw new Exception("Task not found");
        var comment = new Comment
        {
            CreatedAt = DateTime.UtcNow,
            Description = request.Description,
            UserName = request.UserName,
            Task = task
        };

        return await _repository.CreateCommentAsync(comment);
    }
}