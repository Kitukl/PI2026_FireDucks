using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Tasks.Interfaces;

namespace StudyHub.Infrastructure.Repositories;

public class TaskRepository(SDbContext context) : ITaskRepository
{
    public async Task<int> GetCountAsync()
    {
        return await context.Tasks.CountAsync();
    }
}