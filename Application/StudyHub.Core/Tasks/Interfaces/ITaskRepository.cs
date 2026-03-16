using StudyHub.Domain.Entities;

namespace StudyHub.Core.Tasks.Interfaces;

public interface ITaskRepository
{
    Task<Task?> GetTaskAsync(Guid Id);
    Task<List<Task>> GetTasksAsync();
    Task<Guid> AddTaskAsync(Task task);
    Task<Guid> UpdateTaskAsync(Task task);
    Task<Guid> DeleteTaskAsync(Guid Id);
}