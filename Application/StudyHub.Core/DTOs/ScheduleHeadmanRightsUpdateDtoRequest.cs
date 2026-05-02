namespace StudyHub.Core.DTOs
{
    public class ScheduleLeaderRightsUpdateDtoRequest
    {
        public Guid Id { get; set; }
        public bool CanLeaderUpdate { get; set; }
    }
}
