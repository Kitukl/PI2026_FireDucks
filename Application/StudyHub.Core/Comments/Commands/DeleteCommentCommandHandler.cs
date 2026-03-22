using MediatR;
using StudyHub.Core.Comments.Interfaces;

namespace StudyHub.Core.Comments.Commands;

public class DeleteCommentCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
}

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Guid>
{
    private readonly ICommentRepository _repository;

    public DeleteCommentCommandHandler(ICommentRepository repository)
    {
        _repository = repository;
    }
    public async Task<Guid> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteCommentAsync(request.Id);
    }
}