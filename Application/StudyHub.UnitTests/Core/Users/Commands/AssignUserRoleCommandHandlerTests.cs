using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class AssignUserRoleCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;

    public AssignUserRoleCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAssignUserRole_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var userId = Guid.NewGuid();
        var handler = new AssignUserRoleCommandHandler(_repositoryMock.Object);
        var command = new AssignUserRoleCommand { UserId = userId, Role = Role.Admin };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddRole(Role.Admin, userId), Times.Once);
    }
}



