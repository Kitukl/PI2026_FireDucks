using StudyHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.DTOs
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public DayOfWeek Day { get; set; }
        public string LessonType { get; set; }
        public SubjectDto Subject { get; set; }
        public LessonSlotDto LessonSlot { get; set; }

        public List<LecturerDtoResponse> Lecturers { get; set; }
    }
}
