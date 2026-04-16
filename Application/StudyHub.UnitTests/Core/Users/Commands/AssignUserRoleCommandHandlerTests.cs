using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class AssignUserRoleCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAssignRoleToUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        var handler = new AssignUserRoleCommandHandler(repositoryMock.Object);
        var command = new AssignUserRoleCommand { UserId = userId, Role = Role.Admin };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.AddRole(Role.Admin, userId), Times.Once);
    }
}

