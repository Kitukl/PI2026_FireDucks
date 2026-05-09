namespace StudyHub.Infrastructure.Services;

public class SessionTrackingOptions
{
    public const string SectionName = "SessionTracking";

    public int HeartbeatIntervalSeconds { get; set; } = 60;
    public int InactivityTimeoutSeconds { get; set; } = 120;
    public int ExpirationCheckIntervalSeconds { get; set; } = 30;
    public int RetentionMonths { get; set; } = 3;
}
