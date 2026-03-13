namespace StudyHub.Core.DTOs;

public record UsersStatisticDto
(
    DateTime CreatedAt,
    double UserActivityPerMonth,
    int FilesCount
);