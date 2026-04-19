using StudyHub.Core.DTOs;
using DayOfWeek = StudyHub.Domain.Enums.DayOfWeek;

namespace Application.Models
{
    public class ScheduleViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsHeadman { get; set; }
        public bool CanHeadmanUpdate { get; set; }
        public List<LessonSlotDto> UniqueSlots { get; set; } = new();
        public List<DayOfWeek> Days { get; set; } = new();
        public Dictionary<string, List<LessonDto>> Grid { get; set; } = new();
    }
}