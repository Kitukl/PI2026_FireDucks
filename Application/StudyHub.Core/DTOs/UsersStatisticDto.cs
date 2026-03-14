namespace StudyHub.Core.DTOs;

public record UsersStatisticDto
(
    DateTime CreatedAt,
    Dictionary<int,double> UserActivityPerMonth,
    int FileCount
);
