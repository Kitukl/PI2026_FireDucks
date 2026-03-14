using StudyHub.Domain.Entities;

namespace Application.Models;

public class SystemStatisticViewModel
{
    public DateTime CreatedAt {get; set; }
    public double UserActivityPerMonth {get; set; }
    public int FileCount {get; set; }
    public int TaskCount { get; set; }
}