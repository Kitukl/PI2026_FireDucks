using Application.Models;
using StudyHub.Core.DTOs;

namespace StudyHub.Core.Admin.DTOs;

public class AdminSchedulePageDataDto
{
    public DateTime LastGlobalUpdate { get; set; }
    public bool IsAutoUpdateEnabled { get; set; }
    public bool AllowLeadersToUpdate { get; set; }
    public uint AutoUpdateIntervalDays { get; set; }
    public List<GroupDto> Groups { get; set; } = new();
    public Guid? SelectedGroupId { get; set; }
    public DateTime? SelectedGroupLastUpdate { get; set; }
    public ScheduleViewModel? CurrentGroupSchedule { get; set; }
}
