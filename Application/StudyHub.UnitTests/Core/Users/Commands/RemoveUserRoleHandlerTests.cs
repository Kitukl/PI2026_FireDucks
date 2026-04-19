using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class RemoveUserRoleHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;

    public RemoveUserRoleHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldRemoveUserRole_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var userId = Guid.NewGuid();
        var handler = new RemoveUserRoleHandler(_repositoryMock.Object);
        var command = new RemoveUserRoleCommand { UserId = userId, Role = Role.Student };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.RemoveRole(Role.Student, userId), Times.Once);
    }
}



