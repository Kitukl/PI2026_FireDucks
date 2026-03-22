using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Group;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly SDbContext _context;

    public GroupRepository(SDbContext context)
    {
        _context = context;
    }
    public async Task<Group> GetGroupByNameAsync(string groupName)
    {
        return await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupName);
    }
}