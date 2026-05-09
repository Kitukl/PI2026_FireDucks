using StudyHub.Core.DTOs;
using DayOfWeek = StudyHub.Domain.Enums.DayOfWeek;

namespace Application.Models
{
    public class ScheduleViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsLeader { get; set; }
        public bool CanLeaderUpdate { get; set; }
        public List<LessonSlotDto> UniqueSlots { get; set; } = new();
        public List<DayOfWeek> Days { get; set; } = new();
        public Dictionary<string, List<LessonDto>> Grid { get; set; } = new();
    }
}