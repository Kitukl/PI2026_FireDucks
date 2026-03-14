namespace StudyHub.Core.Tasks.Interfaces;

public interface ITaskRepository
{
    Task<int> GetCountAsync();
}