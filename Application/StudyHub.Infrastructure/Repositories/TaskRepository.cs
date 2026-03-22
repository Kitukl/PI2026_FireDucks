using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Enums;
using UserTask = StudyHub.Domain.Entities.Task;

namespace StudyHub.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly SDbContext _context;

    public TaskRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<UserTask> GetTaskAsync(Guid id)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(c => c.Id == id) ?? throw new Exception("Task not found");
        return task;
    }
    public async Task<List<UserTask>> GetTasksAsync()
    {
        return await _context.Tasks.ToListAsync();
    }
    public async Task<Guid> AddTaskAsync(UserTask task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        
        return task.Id;
    }
    public async Task<Guid> UpdateTaskAsync(UserTask task)
    {
        var userTask = await _context.Tasks.FirstOrDefaultAsync(f => f.Id == task.Id) ?? throw new Exception("Task not found");
        
        userTask.Title = task.Title;
        userTask.Status = task.Status;
        userTask.Deadline = task.Deadline;
        userTask.Subject = task.Subject;

        await _context.SaveChangesAsync();

        return userTask.Id;
    }

    public async Task<Guid> DeleteTaskAsync(Guid id)
    {
        await _context.Tasks.Where(f => f.Id == id).ExecuteDeleteAsync();
        return id;
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Tasks.CountAsync();
    }
    
    public async Task<Dictionary<bool, Dictionary<Status, int>>> GetGroupedTaskStatsAsync()    {
        var stats =  _context.Tasks
            .GroupBy(x => new { x.IsGroupTask, x.Status })
            .Select(g => new
            {
                g.Key.IsGroupTask,
                g.Key.Status,
                Count = g.Count()
            });
        
        return stats.GroupBy(s=>s.IsGroupTask)
            .ToDictionary(
                g => g.Key, 
                g => g.ToDictionary(x => x.Status, x => x.Count)
            );
    }
}