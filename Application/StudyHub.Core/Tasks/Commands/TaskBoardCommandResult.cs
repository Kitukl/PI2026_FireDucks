namespace StudyHub.Core.Tasks.Commands;

public class TaskBoardCommandResult
{
    public bool IsSuccess { get; set; }
    public bool IsForbidden { get; set; }
    public bool IsNotFound { get; set; }
    public bool IsNoOp { get; set; }
}
