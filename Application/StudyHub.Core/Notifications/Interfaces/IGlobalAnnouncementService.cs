namespace StudyHub.Core.Notifications.Interfaces;

public interface IGlobalAnnouncementService
{
    Task SendGlobalAnnouncementAsync(
        IReadOnlyCollection<string> recipients,
        string subject,
        string description,
        CancellationToken cancellationToken = default);
}
