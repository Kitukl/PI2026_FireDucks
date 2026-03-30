using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class RemoveUserFromGroupCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldClearGroupAndReturnUserName()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Oleh", Group = new Group { Name = "PI-23" } };
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
        repositoryMock.Setup(x => x.Update(user)).ReturnsAsync(user);

        var handler = new RemoveUserFromGroupCommandHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new RemoveUserFromGroupCommand { UserId = user.Id }, CancellationToken.None);

        // Assert
        Assert.Equal("Oleh", result);
        Assert.Null(user.Group);
        repositoryMock.Verify(x => x.Update(user), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserNotFound_ShouldThrow()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User)null!);
        var handler = new RemoveUserFromGroupCommandHandler(repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new RemoveUserFromGroupCommand { UserId = Guid.NewGuid() }, CancellationToken.None));
    }
}

