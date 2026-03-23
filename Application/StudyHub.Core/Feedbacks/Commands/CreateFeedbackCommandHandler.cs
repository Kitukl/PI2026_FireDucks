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
    public string Description { get; set; } = string.Empty;
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
        var creatorFullname = $"{user.Name} {user.Surname}".Trim();

        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            Category = request.Category,
            CreatorFullname = string.IsNullOrWhiteSpace(creatorFullname) ? user.UserName ?? "User" : creatorFullname,
            Status = request.Status,
            Description = string.IsNullOrWhiteSpace(request.Description) ? string.Empty : request.Description.Trim(),
            FeedbackType = request.FeedbackType,
            CreatedAt = DateTime.UtcNow,
            User = user
        };
        
        return await _repository.AddFeedbackAsync(feedback);
    }
}