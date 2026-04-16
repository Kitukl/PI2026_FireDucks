using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class AddUserRoleCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCallRepositoryAddRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new AddUserRoleCommandHandler(repositoryMock.Object);
        var command = new AddUserRoleCommand { UserId = userId, Role = Role.Leader };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.AddRole(Role.Leader, userId), Times.Once);
    }
}

