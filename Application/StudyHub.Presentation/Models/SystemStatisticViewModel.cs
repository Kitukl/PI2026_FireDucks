using StudyHub.Domain.Entities;

namespace Application.Models;

public class SystemStatisticViewModel
{
    public DateTime CreatedAt {get; set; }
    public Dictionary<int,double> UserActivityPerMonth {get; set; }
    public Dictionary<string,int> UserRoleCount  {get; set; }

    public int FileCount {get; set; }
    public int TaskCount { get; set; }
}