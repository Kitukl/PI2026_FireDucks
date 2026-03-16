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
    
    public async Task<Dictionary<bool, Dictionary<Status, int>>> GetGroupedTaskStatsAsync()    {
        var stats =  context.Tasks
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