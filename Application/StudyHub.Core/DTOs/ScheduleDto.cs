namespace StudyHub.Core.DTOs
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool LeaderUpdate { get; set; }
        public bool IsAutoUpdate { get; set; }
        public uint UpdateInterval { get; set; }
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
        public GroupDto Group { get; set; } = new GroupDto();
    }
}
