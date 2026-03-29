using Moq;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserAlreadyExists_ShouldReturnTrueWithoutCreating()
    {
        // Arrange
        var existingUser = new User { Id = Guid.NewGuid(), Email = "user@test.com" };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(new[] { existingUser });

        var groupRepositoryMock = new Mock<IGroupRepository>();
        var handler = new RegisterUserCommandHandler(userRepositoryMock.Object, groupRepositoryMock.Object);

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
        userRepositoryMock.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);
        userRepositoryMock.Verify(x => x.AddExternalLogin(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        userRepositoryMock.Verify(x => x.AddRole(It.IsAny<Role>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenUserIsNew_ShouldCreateUserAndAssignStudentRole()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(Array.Empty<User>());

        userRepositoryMock
            .Setup(x => x.CreateUser(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var groupRepositoryMock = new Mock<IGroupRepository>();
        var handler = new RegisterUserCommandHandler(userRepositoryMock.Object, groupRepositoryMock.Object);

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
        userRepositoryMock.Verify(x => x.CreateUser(It.Is<User>(u => u.Email == "new@test.com" && u.Name == "New")), Times.Once);
        userRepositoryMock.Verify(x => x.AddExternalLogin(It.IsAny<User>(), "Microsoft", "ms-123"), Times.Once);
        userRepositoryMock.Verify(x => x.AddRole(Role.Student, It.IsAny<Guid>()), Times.Once);
    }
}

