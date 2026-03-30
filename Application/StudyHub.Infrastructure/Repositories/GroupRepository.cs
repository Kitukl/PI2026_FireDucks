using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Group;

namespace StudyHub.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly SDbContext _context;

    public GroupRepository(SDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Group> GetGroupByNameAsync(string groupName)
    {
        return await _context.Groups
            .FirstOrDefaultAsync(g => g.Name == groupName);
    }

    public async Task<List<Domain.Entities.Group>> GetAllGroupsAsync()
    {
        return await _context.Groups.ToListAsync();
    }
}