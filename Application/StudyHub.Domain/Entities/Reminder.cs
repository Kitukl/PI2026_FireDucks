namespace StudyHub.Domain.Entities;

public class Reminder
{
    public Guid Id { get; set; }
    public uint ReminderOffset { get; set; }
    public TimeType TimeType { get; set; }
}