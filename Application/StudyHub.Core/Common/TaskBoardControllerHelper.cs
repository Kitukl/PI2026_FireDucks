using System.Security.Claims;
using StudyHub.Domain.Enums;

namespace Application.Helpers;

public static class TaskBoardControllerHelper
{
    public static Guid? GetCurrentUserId(ClaimsPrincipal user)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }

    public static Status ParseTaskStatus(string rawStatus)
    {
        return rawStatus?.Trim().ToLowerInvariant() switch
        {
            "todo" or "to-do" or "to do" => Status.ToDo,
            "inprogress" or "in-progress" or "in progress" => Status.InProgress,
            "forreview" or "for-review" or "for review" => Status.ForReview,
            "done" => Status.Done,
            "resolved" => Status.Resolved,
            _ => Status.ToDo
        };
    }

    public static string NormalizeTaskStatus(Status status)
    {
        return status switch
        {
            Status.ToDo => "todo",
            Status.InProgress => "in-progress",
            Status.ForReview => "for-review",
            Status.Done => "done",
            Status.Resolved => "done",
            _ => "todo"
        };
    }
}
