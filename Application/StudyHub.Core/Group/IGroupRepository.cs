namespace StudyHub.Core.Group;

public interface IGroupRepository
{
    public Task<Domain.Entities.Group> GetGroupByNameAsync(string groupName);
    public Task<List<Domain.Entities.Group>> GetAllGroupsAsync();
}