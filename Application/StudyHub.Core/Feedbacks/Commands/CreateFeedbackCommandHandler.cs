using MediatR;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Feedbacks.Commands;

public class CreateFeedbackCommand : IRequest<Guid>
{
    public FeedbackType FeedbackType { get; set; }
    public Category Category { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public Guid UserId { get; set; }
}

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, Guid>
{
    private readonly IFeedbackRepository _repository;
    private readonly IUserRepository _userRepository;

    public CreateFeedbackCommandHandler(IFeedbackRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }
    public async Task<Guid> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);
        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            Category = request.Category,
            CreatorFullname = $"{user.Name} {user.Surname}",
            Status = request.Status,
            Description = request.Description,
            FeedbackType = request.FeedbackType,
            CreatedAt = DateTime.UtcNow
        };
        
        return await _repository.AddFeedbackAsync(feedback);
    }
}