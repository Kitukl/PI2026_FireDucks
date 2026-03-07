namespace StudyHub.Domain.Entities;

public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<User> Users { get; set; }
}