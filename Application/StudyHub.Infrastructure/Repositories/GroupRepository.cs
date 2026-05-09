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

    public async Task<Group?> GetGroupByNameAsync(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return null;
        }

        var normalizedGroupName = groupName.Trim();

        return await _context.Groups
            .FirstOrDefaultAsync(g => g.Name == normalizedGroupName);
    }
    
    public async Task<Group?> GetGroupByIdAsync(Guid groupId)
    {
        return await _context.Groups
            .Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<Group> CreateGroupAsync(string groupName)
    {
        var normalizedGroupName = groupName.Trim();

        var existingGroup = await GetGroupByNameAsync(normalizedGroupName);
        if (existingGroup != null)
        {
            return existingGroup;
        }

        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = normalizedGroupName
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return group;
    }

    public async Task<bool> UpdateGroupAsync(Guid id, string name)
    {
        var updated = await _context.Groups
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Name, name));
        
        await _context.SaveChangesAsync();
        return updated > 0;
    }

    public async Task<bool> DeleteGroupAsync(Guid id)
    {
        var deleted = await _context.Groups
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
        return deleted > 0;
    }

    public async Task<List<Group>> GetAllGroupsAsync()
    {
        return await _context.Groups.ToListAsync();
    }
}