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
    private const int CommentDescriptionMaxLength = 1000;
    private const int CommentUserNameMaxLength = 100;

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
        var normalizedDescription = request.Description?.Trim() ?? string.Empty;
        if (normalizedDescription.Length > CommentDescriptionMaxLength)
        {
            normalizedDescription = normalizedDescription[..CommentDescriptionMaxLength];
        }

        var normalizedUserName = string.IsNullOrWhiteSpace(request.UserName)
            ? "Guest"
            : request.UserName.Trim();
        if (normalizedUserName.Length > CommentUserNameMaxLength)
        {
            normalizedUserName = normalizedUserName[..CommentUserNameMaxLength];
        }

        var comment = new Comment
        {
            CreatedAt = DateTime.UtcNow,
            Description = normalizedDescription,
            UserName = normalizedUserName,
            Task = task
        };

        return await _repository.CreateCommentAsync(comment);
    }
}