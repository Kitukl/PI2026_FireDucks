using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly StudyHubDbContext context;

    public TaskRepository(StudyHubDbContext context)
    {
        _context = context;
    }

    public async Task<Task?> GetTaskAsync(Guid Id)
    {
        return await _context.Tasks.FindAsync(Id);
    }
    public async Task<List<Task>> GetTasksAsync()
    {
        return await _context.Tasks.ToListAsync();
    }
    public async Task<Guid> AddTaskAsync(Task task)
    {
        await _context.Tasks.AddAsync(task);
        return task.Id;
    }
    public async Task<Guid> UpdateTaskAsync(Task task)
    {
        var userTask = await _context.Task.FirstOrDefaultAsync(f => f.Id == task.Id);

        if (userTask != null)
        {
            userTask.Title = task.Title;
            userTask.Status = task.Status;
            userTask.Deadline = task.Deadline;
            userTask.Subject = task.Subject;

            await _context.SaveChangesAsync();
        }

        return userTask.Id.ToString();
    }

    public async Task<Guid> DeleteTaskAsync(Guid Id)
    {
        return await _context.Tasks
            .Where(f => f.Id == task.Id)
            .ExecuteDeleteAsync();
    }

}