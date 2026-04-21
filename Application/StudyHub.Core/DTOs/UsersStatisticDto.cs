namespace StudyHub.Core.DTOs;

public record UsersStatisticDto
(
    DateTime CreatedAt,
    Dictionary<int,double> UserActivityPerMonth,
    int UserFilesCount,
    int GroupFilesCount,
    int StudentsCount,
    int GroupsCount,
    int LeadersCount
)
{
    public int FileCount => UserFilesCount + GroupFilesCount;
}
