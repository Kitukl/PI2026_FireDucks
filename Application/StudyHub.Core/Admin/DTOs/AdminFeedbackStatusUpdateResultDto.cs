namespace StudyHub.Core.Admin.DTOs;

public class AdminFeedbackStatusUpdateResultDto
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
}
