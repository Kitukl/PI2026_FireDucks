using StudyHub.Core.Common;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Common.Helpers;

public class StorageScopeHelperTests
{
    [Fact]
    public void ResolveContainerName_ShouldReturnGroupContainer_WhenScopeIsGroupAndGroupExists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Group = new Group { Name = "PI-24" }
        };

        // Act
        var result = StorageScopeHelper.ResolveContainerName(user, "group");

        // Assert
        Assert.Equal("group-storage-PI-24", result);
    }

    [Fact]
    public void ResolveContainerName_ShouldReturnNull_WhenScopeIsGroupAndGroupIsMissing()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Group = null
        };

        // Act
        var result = StorageScopeHelper.ResolveContainerName(user, "group");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ResolveContainerName_ShouldReturnUserContainer_WhenScopeIsPersonal()
    {
        // Arrange
        var user = new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222") };

        // Act
        var result = StorageScopeHelper.ResolveContainerName(user, "personal");

        // Assert
        Assert.Equal("user-storage-22222222-2222-2222-2222-222222222222", result);
    }
}
