namespace StudyHub.Domain.Entities;

public class Lesson
{
    public Guid Id { get; set; }
    
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    
    public ICollection<Schedule> Schedules { get; set; }
    public ICollection<Lecturer> Lectures { get; set; }
    public ICollection<LessonsSlot> LessonsSlots { get; set; }
}