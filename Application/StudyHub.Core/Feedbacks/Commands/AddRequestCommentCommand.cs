using MediatR;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Core.Feedbacks.Commands;

public class AddRequestCommentCommand : IRequest<RequestCommentCommandResult>
{
    public Guid? CurrentUserId { get; set; }
    public Guid FeedbackId { get; set; }
    public string? Description { get; set; }
    public bool IsAdmin { get; set; }
}

public class AddRequestCommentCommandHandler : IRequestHandler<AddRequestCommentCommand, RequestCommentCommandResult>
{
    private const int CommentDescriptionMaxLength = 1000;

    private readonly ICommentRepository _commentRepository;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IUserRepository _userRepository;

    public AddRequestCommentCommandHandler(ICommentRepository commentRepository, IFeedbackRepository feedbackRepository, IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _feedbackRepository = feedbackRepository;
        _userRepository = userRepository;
    }

    public async Task<RequestCommentCommandResult> Handle(AddRequestCommentCommand request, CancellationToken cancellationToken)
    {
        if (!request.CurrentUserId.HasValue)
        {
            return new RequestCommentCommandResult { IsForbidden = true };
        }

        var feedback = await _feedbackRepository.GetFeedbackAsync(request.FeedbackId);
        if (feedback == null)
        {
            return new RequestCommentCommandResult { IsNotFound = true };
        }

        if (!request.IsAdmin && feedback.User.Id != request.CurrentUserId.Value)
        {
            return new RequestCommentCommandResult { IsForbidden = true };
        }

        var normalizedDescription = request.Description?.Trim() ?? string.Empty;
        if (normalizedDescription.Length > CommentDescriptionMaxLength)
        {
            normalizedDescription = normalizedDescription[..CommentDescriptionMaxLength];
        }

        if (string.IsNullOrWhiteSpace(normalizedDescription))
        {
            return new RequestCommentCommandResult { IsNotFound = false, IsSuccess = false };
        }

        var currentUser = await _userRepository.GetUserById(request.CurrentUserId.Value);
        var authorName = request.IsAdmin
            ? "Admin"
            : BuildAuthorName(currentUser);

        var comment = new Comment
        {
            CreatedAt = DateTime.UtcNow,
            Description = normalizedDescription,
            UserName = authorName,
            Feedback = feedback
        };

        await _commentRepository.CreateCommentAsync(comment);
        return new RequestCommentCommandResult { IsSuccess = true };
    }

    private static string BuildAuthorName(User user)
    {
        var fullName = $"{user.Name} {user.Surname}".Trim();
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            return fullName;
        }

        return string.IsNullOrWhiteSpace(user.UserName) ? "User" : user.UserName.Trim();
    }
}