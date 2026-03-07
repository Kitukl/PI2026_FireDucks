namespace StudyHub.Domain.Entitties;

public class Subject
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<Lesson> Lessons { get; set; }
}