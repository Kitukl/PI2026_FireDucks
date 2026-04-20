namespace StudyHub.Core.Admin.DTOs;

public class AdminDashboardDataDto
{
    public DateTime CreatedAt { get; set; }
    public Dictionary<int, double> UserActivityPerMonth { get; set; } = new();
    public int StudentsCount { get; set; }
    public int GroupsCount { get; set; }
    public int LeadersCount { get; set; }
    public int UserFilesCount { get; set; }
    public int GroupFilesCount { get; set; }
    public int FileCount { get; set; }
    public int TaskCount { get; set; }
    public Dictionary<string, Dictionary<string, int>> GroupedTaskCount { get; set; } = new();
}
