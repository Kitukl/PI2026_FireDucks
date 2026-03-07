namespace StudyHub.Domain.Entitties;

public class Lecturer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    public ICollection<Lesson> Lessons { get; set; }
}