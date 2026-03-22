using MediatR;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Comments.Queries;

public class GetCommentsQuery : IRequest<List<Comment>>
{
    public Guid TaskId { get; set; }
}

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<Comment>>
{
    private readonly ICommentRepository _repository;

    public GetCommentsQueryHandler(ICommentRepository repository)
    {
        _repository = repository;
    }
    public async Task<List<Comment>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCommentsAsync(request.TaskId);
    }
}