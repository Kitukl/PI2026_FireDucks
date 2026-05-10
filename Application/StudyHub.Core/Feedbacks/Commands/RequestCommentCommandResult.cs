namespace StudyHub.Core.Feedbacks.Commands;

public class RequestCommentCommandResult
{
    public bool IsSuccess { get; set; }
    public bool IsForbidden { get; set; }
    public bool IsNotFound { get; set; }
}