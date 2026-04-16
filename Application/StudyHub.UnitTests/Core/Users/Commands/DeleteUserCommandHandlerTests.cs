using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class DeleteUserCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteUserAndReturnUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new DeleteUserCommandHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new DeleteUserCommand { UserId = userId }, CancellationToken.None);

        // Assert
        Assert.Equal(userId, result);
        repositoryMock.Verify(x => x.Delete(userId), Times.Once);
    }
}

