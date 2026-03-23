namespace StudyHub.Core.DTOs
{
    public class LecturerDtoRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<LessonDto>? Lessons { get; set; } = new List<LessonDto>();
    }
}
