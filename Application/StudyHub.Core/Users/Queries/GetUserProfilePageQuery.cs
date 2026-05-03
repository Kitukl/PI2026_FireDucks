using Application.Helpers;
using MediatR;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using TimeType = StudyHub.Domain.Entities.TimeType;

namespace StudyHub.Core.Users.Queries;

public class GetUserProfilePageQuery : IRequest<UserProfilePageDataDto>
{
    public Guid? UserId { get; set; }
    public string? FeedbackId { get; set; }
    public bool OpenModal { get; set; }
}

public class UserProfilePageDataDto
{
    public string FullName { get; set; } = "Гість";
    public string PhotoUrl { get; set; } = "/images/no-photo.png";
    public bool IsNotified { get; set; }
    public uint ReminderOffset { get; set; } = 2u;
    public TimeType ReminderTimeType { get; set; } = TimeType.Day;
    public List<Feedback> Requests { get; set; } = [];
    public Feedback? ActiveRequest { get; set; }
    public List<Comment> ActiveRequestComments { get; set; } = [];
    public bool OpenRequestModal { get; set; }
}

public class GetUserProfilePageQueryHandler : IRequestHandler<GetUserProfilePageQuery, UserProfilePageDataDto>
{
    private readonly ISender _sender;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;

    public GetUserProfilePageQueryHandler(ISender sender, IUserRepository userRepository, ICommentRepository commentRepository)
    {
        _sender = sender;
        _userRepository = userRepository;
        _commentRepository = commentRepository;
    }

    public async Task<UserProfilePageDataDto> Handle(GetUserProfilePageQuery request, CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue)
        {
            return new UserProfilePageDataDto();
        }

        var user = await _userRepository.GetUserById(request.UserId.Value);
        var feedbacks = await _sender.Send(new GetFeedbacksCommand(), cancellationToken);
        var requests = feedbacks
            .Where(feedback => feedback.User?.Id == request.UserId.Value && feedback.FeedbackType == FeedbackType.Request)
            .OrderByDescending(feedback => feedback.CreatedAt)
            .ToList();

        Feedback? activeRequest = null;
        if (!string.IsNullOrWhiteSpace(request.FeedbackId) && Guid.TryParse(request.FeedbackId, out var parsedFeedbackId))
        {
            activeRequest = requests.FirstOrDefault(feedback => feedback.Id == parsedFeedbackId);
        }

        activeRequest ??= requests.FirstOrDefault();

        var activeRequestComments = new List<Comment>();
        if (request.OpenModal && activeRequest != null)
        {
            activeRequestComments = await _commentRepository.GetFeedbackCommentsAsync(activeRequest.Id);
        }

        return new UserProfilePageDataDto
        {
            FullName = $"{user.Name} {user.Surname}".Trim(),
            PhotoUrl = UserProfileHelper.ResolvePhotoUrl(user.PhotoUrl),
            IsNotified = user.IsNotified,
            ReminderOffset = user.Reminder?.ReminderOffset ?? 2u,
            ReminderTimeType = user.Reminder?.TimeType ?? TimeType.Day,
            Requests = requests,
            ActiveRequest = activeRequest,
            ActiveRequestComments = activeRequestComments,
            OpenRequestModal = request.OpenModal && activeRequest != null
        };
    }
}
