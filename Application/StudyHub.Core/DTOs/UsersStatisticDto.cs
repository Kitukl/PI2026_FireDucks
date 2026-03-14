namespace StudyHub.Core.DTOs;

public record UsersStatisticDto
(
    DateTime CreatedAt,
    List<(int,double)> UserActivityPerMonth,
    int FileCount
);
