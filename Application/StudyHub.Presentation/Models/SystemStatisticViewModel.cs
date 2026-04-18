using StudyHub.Domain.Entities;

namespace Application.Models;

public class SystemStatisticViewModel
{
    public DateTime CreatedAt {get; set; }
    public Dictionary<int,double> UserActivityPerMonth {get; set; }
    public Dictionary<string,int> UserRoleCount  {get; set; }

    public int StudentsCount { get; set; }
    public int GroupsCount { get; set; }
    public int LeadersCount { get; set; }

    public int UserFilesCount { get; set; }
    public int GroupFilesCount { get; set; }
    public int FileCount {get; set; }

    public int TaskCount { get; set; }
    public Dictionary<string, Dictionary<string, int>> GropedTaskCount { get; set; }
}