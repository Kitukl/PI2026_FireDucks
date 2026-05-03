using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace Application.Models;

public class AdminRequestsViewModel
{
    public List<Feedback> Requests { get; set; } = [];
    public Feedback? ActiveRequest { get; set; }
    public List<Comment> ActiveRequestComments { get; set; } = [];
    public bool OpenRequestModal { get; set; }

    public List<Status> AllowedStatuses { get; } =
    [
        Status.ToDo,
        Status.InProgress,
        Status.Resolved
    ];
}
