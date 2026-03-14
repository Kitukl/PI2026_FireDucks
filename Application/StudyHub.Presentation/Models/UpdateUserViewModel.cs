namespace Application.Models;

public class UpdateUserViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string Surname { get; set; }

    public string GroupName { get; set; }

    public List<string> Roles { get; set; } = new();

    public List<string> AvailableRoles { get; set; } = new();
}