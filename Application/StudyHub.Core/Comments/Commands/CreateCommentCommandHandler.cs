using MediatR;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Domain.Entities;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Comments.Commands;

public class CreateCommentCommand : IRequest<Comment>
{
    public string UserName { get; set; }
    public string Description { get; set; }
    public Task Task { get; set; }
}

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Comment>
{
    private readonly ICommentRepository _repository;

    public CreateCommentCommandHandler(ICommentRepository repository)
    {
        _repository = repository;
    }
    public async Task<Comment> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        return new Comment();
    }
}