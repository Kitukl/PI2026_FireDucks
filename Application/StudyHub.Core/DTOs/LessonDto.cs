namespace StudyHub.Core.DTOs
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public DayOfWeek Day { get; set; }
        public string? LessonType { get; set; }
        public string? Room { get; set; }
        public SubjectDto Subject { get; set; } = new SubjectDto();
        public LessonSlotDto LessonSlot { get; set; } = new LessonSlotDto();

        public List<LecturerDtoResponse>? Lecturers { get; set; } = new List<LecturerDtoResponse>();
    }
}
