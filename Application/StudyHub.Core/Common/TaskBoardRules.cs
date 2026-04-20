namespace Application.Helpers;

public static class TaskBoardRules
{
    public const int TaskTitleMaxLength = 200;
    public const int SummaryMaxLength = 200;
    public const int SubjectDisplayMaxLength = 34;
    public const int CommentMaxLength = 1000;
    public const int CommentUserNameMaxLength = 100;
    public const int MaxDueDateYearsFromToday = 5;

    public static DateTime GetMaxDueDate()
    {
        return DateTime.Today.AddYears(MaxDueDateYearsFromToday);
    }
}
