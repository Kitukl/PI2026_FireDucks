namespace StudyHub.Domain.Entities;

public class LessonsSlot
{
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public ICollection<Lesson>? Lessons { get; set; }
}