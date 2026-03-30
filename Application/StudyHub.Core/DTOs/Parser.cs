using System.Collections.Generic;

namespace StudyHub.Core.DTOs.Parser
{
    public class ParsedScheduleResponse
    {
        public string GroupName { get; set; }
        public List<ParsedLessonDto> Lessons { get; set; } = new();
    }

    public class ParsedLessonDto
    {
        public int Day { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string SubjectName { get; set; }
        public string LessonType { get; set; }
        public string Room { get; set; }
        public List<ParsedTeacherDto> Teachers { get; set; } = new();
    }

    public class ParsedTeacherDto
    {
        public string Surname { get; set; }
        public string Name { get; set; }
    }
}