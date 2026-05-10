using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class TaskBoardCreatePageViewModel
{
    public TaskBoardPageViewModel Board { get; set; } = new();
    public List<SubjectOptionViewModel> Subjects { get; set; } = [];

    [Required]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public Guid? SubjectId { get; set; }

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);

    public bool IsGroupTask { get; set; } = true;
    public string? ResourceUrl { get; set; }
}
