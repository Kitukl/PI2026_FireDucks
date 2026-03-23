using MediatR;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Feedbacks.Queries;

public class GetFeedbackCommand : IRequest<Feedback>
{
    public Guid Id { get; set; }
}

public class GetFeedbackCommandHandler : IRequestHandler<GetFeedbackCommand, Feedback>
{
    private readonly IFeedbackRepository _repository;

    public GetFeedbackCommandHandler(IFeedbackRepository repository)
    {
        _repository = repository;
    }
    public async Task<Feedback> Handle(GetFeedbackCommand request, CancellationToken cancellationToken)
    {
        return await _repository.GetFeedbackAsync(request.Id) ?? throw new Exception("Feedback not found");
    }
}