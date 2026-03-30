namespace StudyHub.Domain.Entities;

public class Lesson
{
    public Guid Id { get; set; }
    public DayOfWeek Day { get; set; }
    public string LessonType { get; set; }
    public Subject Subject { get; set; }
    public LessonsSlot LessonsSlot { get; set; }
    public string? Room { get; set; }

    public ICollection<Schedule> Schedules { get; set; }
    public ICollection<Lecturer> Lecturers { get; set; }
}