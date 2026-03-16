using StudyHub.Domain.Entities;

namespace StudyHub.Core.Tasks.Interfaces;

public interface ITaskRepository
{
    Task<int> GetCountAsync();
    Task<Dictionary<bool, Dictionary<Status, int>>> GetGroupedTaskStatsAsync();
}