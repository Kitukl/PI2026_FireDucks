namespace StudyHub.Core.Group;

public interface IGroupRepository
{
    public Task<Domain.Entities.Group?> GetGroupByNameAsync(string groupName);
    public Task<Domain.Entities.Group?> GetGroupByIdAsync(Guid groupId);
    public Task<Domain.Entities.Group> CreateGroupAsync(string groupName);
    public Task<bool> UpdateGroupAsync(Guid id, string name);
    public Task<bool> DeleteGroupAsync(Guid id);
    public Task<List<Domain.Entities.Group>> GetAllGroupsAsync();
}