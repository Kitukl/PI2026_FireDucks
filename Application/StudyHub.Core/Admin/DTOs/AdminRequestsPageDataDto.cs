using StudyHub.Domain.Entities;

namespace StudyHub.Core.Admin.DTOs;

public class AdminRequestsPageDataDto
{
    public List<Feedback> Requests { get; set; } = new();
    public Feedback? ActiveRequest { get; set; }
    public bool OpenRequestModal { get; set; }
}
