using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class TaskRepository(SDbContext context) : ITaskRepository
{
    public async Task<int> GetCountAsync()
    {
        return await context.Tasks.CountAsync();
    }
    
    public Dictionary<Status,int> GetCountByStatusAsync()
    {
        return context.Tasks
            .GroupBy(t => t.Status)
            .ToDictionary(
                g =>g.Key,
                g=> g.Count());
    }
}