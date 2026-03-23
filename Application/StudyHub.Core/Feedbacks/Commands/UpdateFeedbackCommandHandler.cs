using MediatR;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Feedbacks.Commands;

public class UpdateFeedbackCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Status Status { get; set; }
}

public class UpdateFeedbackCommandHandler : IRequestHandler<UpdateFeedbackCommand, Guid>
{
    private readonly IFeedbackRepository _feedbackRepository;

    public UpdateFeedbackCommandHandler(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }
    public async Task<Guid> Handle(UpdateFeedbackCommand request, CancellationToken cancellationToken)
    {
        var feedback = await _feedbackRepository.GetFeedbackAsync(request.Id) ?? throw new Exception("Feedback not found");
        feedback.Status = request.Status;
        feedback.UpdatedAt = DateTime.UtcNow;

        if (request.Status == Status.Resolved)
        {
            feedback.ResolvedAt = DateTime.UtcNow;
        }
        else
        {
            feedback.ResolvedAt = null;
        }

        return await _feedbackRepository.UpdateFeedbackAsync(feedback);
    }
}