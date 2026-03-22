namespace StudyHub.Core.DTOs
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool HeadmanUpdate { get; set; }
        public bool IsAutoUpdate { get; set; }
        public List<LessonDto> Lessons { get; set; }
        public GroupDto Group { get; set; }
    }
}
