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

    public async Task<Domain.Entities.Group?> GetGroupByNameAsync(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return null;
        }

        var normalizedGroupName = groupName.Trim();

        return await _context.Groups
            .FirstOrDefaultAsync(g => g.Name == normalizedGroupName);
    }

    public async Task<Domain.Entities.Group> CreateGroupAsync(string groupName)
    {
        var normalizedGroupName = groupName.Trim();

        var existingGroup = await GetGroupByNameAsync(normalizedGroupName);
        if (existingGroup != null)
        {
            return existingGroup;
        }

        var group = new Domain.Entities.Group
        {
            Id = Guid.NewGuid(),
            Name = normalizedGroupName
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return group;
    }

    public async Task<List<Domain.Entities.Group>> GetAllGroupsAsync()
    {
        return await _context.Groups.ToListAsync();
    }
}