namespace Application.Models
{
    public class AdminScheduleViewModel
    {
        public DateTime LastGlobalUpdate { get; set; }
        public bool IsAutoUpdateEnabled { get; set; }
        public bool AllowLeadersToUpdate { get; set; }
        public uint AutoUpdateIntervalDays { get; set; }
        public List<StudyHub.Core.DTOs.GroupDto> Groups { get; set; } = new();
        public Guid? SelectedGroupId { get; set; }
        public Application.Models.ScheduleViewModel CurrentGroupSchedule { get; set; }
    }
}