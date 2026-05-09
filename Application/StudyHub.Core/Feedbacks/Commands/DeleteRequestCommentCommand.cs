using MediatR;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Feedbacks.Commands;

public class DeleteRequestCommentCommand : IRequest<RequestCommentCommandResult>
{
    public Guid? CurrentUserId { get; set; }
    public Guid FeedbackId { get; set; }
    public Guid CommentId { get; set; }
    public bool IsAdmin { get; set; }
}

public class DeleteRequestCommentCommandHandler : IRequestHandler<DeleteRequestCommentCommand, RequestCommentCommandResult>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IUserRepository _userRepository;

    public DeleteRequestCommentCommandHandler(ICommentRepository commentRepository, IFeedbackRepository feedbackRepository, IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _feedbackRepository = feedbackRepository;
        _userRepository = userRepository;
    }

    public async Task<RequestCommentCommandResult> Handle(DeleteRequestCommentCommand request, CancellationToken cancellationToken)
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

        var comments = await _commentRepository.GetFeedbackCommentsAsync(request.FeedbackId);
        var targetComment = comments.FirstOrDefault(comment => comment.Id == request.CommentId);
        if (targetComment == null)
        {
            return new RequestCommentCommandResult { IsNotFound = true };
        }

        if (!request.IsAdmin)
        {
            var currentUser = await _userRepository.GetUserById(request.CurrentUserId.Value);
            var authorName = BuildAuthorName(currentUser);
            if (!string.Equals(targetComment.UserName, authorName, StringComparison.OrdinalIgnoreCase))
            {
                return new RequestCommentCommandResult { IsForbidden = true };
            }
        }

        await _commentRepository.DeleteCommentAsync(request.CommentId);
        return new RequestCommentCommandResult { IsSuccess = true };
    }

    private static string BuildAuthorName(StudyHub.Domain.Entities.User user)
    {
        var fullName = $"{user.Name} {user.Surname}".Trim();
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            return fullName;
        }

        return string.IsNullOrWhiteSpace(user.UserName) ? "User" : user.UserName.Trim();
    }
}