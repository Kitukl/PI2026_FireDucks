namespace StudyHub.Core.Group;

public interface IGroupRepository
{
    public Task<Domain.Entities.Group> GetGroupByNameAsync(string groupName);
}