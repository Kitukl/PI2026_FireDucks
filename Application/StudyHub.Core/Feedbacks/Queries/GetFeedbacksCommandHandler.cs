using MediatR;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Feedbacks.Queries;

public class GetFeedbacksCommand : IRequest<List<Feedback>>;

public class GetFeedbacksCommandHandler : IRequestHandler<GetFeedbacksCommand, List<Feedback>>
{
    private readonly IFeedbackRepository _repository;

    public GetFeedbacksCommandHandler(IFeedbackRepository repository)
    {
        _repository = repository;
    }
    public async Task<List<Feedback>> Handle(GetFeedbacksCommand request, CancellationToken cancellationToken)
    {
        return await _repository.GetFeedbacksAsync();
    }
}