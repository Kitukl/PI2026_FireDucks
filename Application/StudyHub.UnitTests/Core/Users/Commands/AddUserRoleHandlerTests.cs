using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class AddUserRoleCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;

    public AddUserRoleCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrang
        var userId = Guid.NewGuid();
        var handler = new AddUserRoleCommandHandler(_repositoryMock.Object);
        var command = new AddUserRoleCommand { UserId = userId, Role = Role.Leader };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddRole(Role.Leader, userId), Times.Once);
    }
}

