using Moq;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public RegisterUserCommandHandlerTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _groupRepositoryMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        var existingUser = new User { Id = Guid.NewGuid(), Email = "user@test.com" };

        _userRepositoryMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(new[] { existingUser });
        _userRepositoryMock.Setup(x => x.GetRolesByUser(existingUser)).ReturnsAsync(new List<string> { "Student" });

        var handler = new RegisterUserCommandHandler(_userRepositoryMock.Object, _groupRepositoryMock.Object);

        var command = new RegisterUserCommand
        {
            Email = "user@test.com",
            Name = "Name",
            Surname = "Surname",
            ProviderName = "Microsoft",
            MicrosoftId = "ms-id"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);
        _userRepositoryMock.Verify(x => x.AddExternalLogin(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.AddRole(It.IsAny<Role>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_2()
    {
        _groupRepositoryMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(Array.Empty<User>());

        _userRepositoryMock
        .Setup(x => x.CreateUser(It.IsAny<User>()))
        .ReturnsAsync((User u) => u);

        var handler = new RegisterUserCommandHandler(_userRepositoryMock.Object, _groupRepositoryMock.Object);

        var command = new RegisterUserCommand
        {
            Email = "new@test.com",
            Name = "New",
            Surname = "User",
            ProviderName = "Microsoft",
            MicrosoftId = "ms-123"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _userRepositoryMock.Verify(x => x.CreateUser(It.Is<User>(u => u.Email == "new@test.com" && u.Name == "New")), Times.Once);
        _userRepositoryMock.Verify(x => x.AddExternalLogin(It.IsAny<User>(), "Microsoft", "ms-123"), Times.Once);
        _userRepositoryMock.Verify(x => x.AddRole(Role.Student, It.IsAny<Guid>()), Times.Once);
    }
}

