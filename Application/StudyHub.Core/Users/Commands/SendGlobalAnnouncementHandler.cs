using MediatR;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.Core.Users.Commands;

public class SendGlobalAnnouncementCommand : IRequest<int>
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class SendGlobalAnnouncementCommandHandler : IRequestHandler<SendGlobalAnnouncementCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IGlobalAnnouncementService _globalAnnouncementService;

    public SendGlobalAnnouncementCommandHandler(
        IUserRepository userRepository,
        IGlobalAnnouncementService globalAnnouncementService)
    {
        _userRepository = userRepository;
        _globalAnnouncementService = globalAnnouncementService;
    }

    public async Task<int> Handle(SendGlobalAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var subject = request.Subject.Trim();
        var description = request.Description.Trim();

        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(description))
        {
            return 0;
        }

        var recipients = await _userRepository.GetAllEmailsAsync();
        if (recipients.Count == 0)
        {
            return 0;
        }

        await _globalAnnouncementService.SendGlobalAnnouncementAsync(
            recipients,
            subject,
            description,
            cancellationToken);

        return recipients.Count;
    }
}
