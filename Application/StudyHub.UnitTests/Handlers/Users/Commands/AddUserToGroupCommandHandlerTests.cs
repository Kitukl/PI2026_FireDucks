using Moq;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class AddUserToGroupCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAssignGroupAndReturnGroupName()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Nazar" };
        var group = new Group { Id = Guid.NewGuid(), Name = "PI-24" };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
        userRepositoryMock.Setup(x => x.Update(user)).ReturnsAsync(user);

        var groupRepositoryMock = new Mock<IGroupRepository>();
        groupRepositoryMock.Setup(x => x.GetGroupByNameAsync("PI-24")).ReturnsAsync(group);

        var handler = new AddUserToGroupCommandHandler(userRepositoryMock.Object, groupRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new AddUserToGroupCommand { UserId = user.Id, GroupName = "PI-24" }, CancellationToken.None);

        // Assert
        Assert.Equal("PI-24", result);
        Assert.Equal(group, user.Group);
        userRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }
}

