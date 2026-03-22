namespace StudyHub.Core.DTOs
{
    public class LessonSlotDto
    {
        public Guid Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public List<SubjectDto> Lessons { get; set; }
    }
}