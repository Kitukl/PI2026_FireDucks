using StudyHub.Domain.Enums;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Tasks.Interfaces;

public interface ITaskRepository
{
    Task<Task?> GetTaskAsync(Guid Id);
    Task<List<Task>> GetTasksAsync();
    Task<Guid> AddTaskAsync(Task task);
    Task<Guid> UpdateTaskAsync(Task task);
    Task<Guid> DeleteTaskAsync(Guid Id);
    Task<int> GetCountAsync();
    Task<Dictionary<bool, Dictionary<Status, int>>> GetGroupedTaskStatsAsync();
}