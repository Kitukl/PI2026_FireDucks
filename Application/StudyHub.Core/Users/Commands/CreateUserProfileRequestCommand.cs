using Application.Helpers;
using MediatR;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.Users.Commands;

public class CreateUserProfileRequestCommand : IRequest<CreateUserProfileRequestResult>
{
    public Guid? UserId { get; set; }
    public FeedbackType FeedbackType { get; set; }
    public Category Category { get; set; }
    public string? Subject { get; set; }
    public string? Description { get; set; }
}

public class CreateUserProfileRequestResult
{
    public bool IsForbidden { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}

public class CreateUserProfileRequestCommandHandler : IRequestHandler<CreateUserProfileRequestCommand, CreateUserProfileRequestResult>
{
    private readonly IUserRepository _userRepository;
    private readonly ISender _sender;

    public CreateUserProfileRequestCommandHandler(IUserRepository userRepository, ISender sender)
    {
        _userRepository = userRepository;
        _sender = sender;
    }

    public async Task<CreateUserProfileRequestResult> Handle(CreateUserProfileRequestCommand request, CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue)
        {
            return new CreateUserProfileRequestResult { IsForbidden = true };
        }

        var user = await _userRepository.GetUserById(request.UserId.Value);

        var fullDescription = UserProfileHelper.BuildFeedbackDescription(request.Subject, request.Description);
        if (string.IsNullOrWhiteSpace(fullDescription))
        {
            return new CreateUserProfileRequestResult
            {
                ErrorMessage = "Please fill in request details before sending."
            };
        }

        await _sender.Send(new CreateFeedbackCommand
        {
            UserId = user.Id,
            FeedbackType = request.FeedbackType,
            Category = request.Category,
            Description = fullDescription,
            Status = Status.ToDo
        }, cancellationToken);

        return new CreateUserProfileRequestResult
        {
            IsSuccess = true,
            SuccessMessage = "Request sent."
        };
    }
}
