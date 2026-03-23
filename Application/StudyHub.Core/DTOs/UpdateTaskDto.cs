using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.Core.DTOs;

public class UpdateTaskDto
{
    public Status Status { get; set; }
    public Subject Subject { get; set; }
}