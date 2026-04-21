namespace Application.Models;

public class UpdateUserViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PhotoUrl { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;

    public string GroupName { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();

    public List<string> SelectedRoles { get; set; } = new();

    public List<string> AvailableRoles { get; set; } = new();

    public List<string> ExistingGroups { get; set; } = new();
}