using StudyHub.Domain.Entities;
using Task = StudyHub.Domain.Entities.Task;

namespace StudyHub.Core.Tasks.Interfaces;

public interface ITaskRepository
{
    System.Threading.Tasks.Task<Task?> GetTaskAsync(Guid Id);
    System.Threading.Tasks.Task<List<Task>> GetTasksAsync();
    System.Threading.Tasks.Task<Guid> AddTaskAsync(Task task);
    System.Threading.Tasks.Task<Guid> UpdateTaskAsync(Task task);
    System.Threading.Tasks.Task<Guid> DeleteTaskAsync(Guid Id);
}