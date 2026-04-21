using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;

    public DeleteUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteUser_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var userId = Guid.NewGuid();
        var handler = new DeleteUserCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new DeleteUserCommand { UserId = userId }, CancellationToken.None);

        // Assert
        Assert.Equal(userId, result);
        _repositoryMock.Verify(x => x.Delete(userId), Times.Once);
    }
}



