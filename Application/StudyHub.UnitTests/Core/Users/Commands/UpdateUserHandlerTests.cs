using Moq;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class UpdateUserHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldSetGroupAndUpdateUser()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Ira" };
        var group = new Group { Name = "PI-25" };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
        userRepositoryMock.Setup(x => x.Update(user)).ReturnsAsync(user);

        var groupRepositoryMock = new Mock<IGroupRepository>();
        groupRepositoryMock.Setup(x => x.GetGroupByNameAsync("PI-25")).ReturnsAsync(group);

        var handler = new UpdateUserHandler(userRepositoryMock.Object, groupRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new UpdateUserCommand { Id = user.Id, GroupName = "PI-25" }, CancellationToken.None);

        // Assert
        Assert.Equal(group, result.Group);
        userRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserNotFound_ShouldThrow()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User)null!);

        var groupRepositoryMock = new Mock<IGroupRepository>();
        var handler = new UpdateUserHandler(userRepositoryMock.Object, groupRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new UpdateUserCommand { Id = Guid.NewGuid(), GroupName = "PI-25" }, CancellationToken.None));
    }
}

