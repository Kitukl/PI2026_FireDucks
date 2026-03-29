using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class RemoveUserRoleHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldRemoveRoleFromUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new RemoveUserRoleHandler(repositoryMock.Object);
        var command = new RemoveUserRoleCommand { UserId = userId, Role = Role.Student };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.RemoveRole(Role.Student, userId), Times.Once);
    }
}

