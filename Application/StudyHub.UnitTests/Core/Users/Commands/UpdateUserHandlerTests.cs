using Moq;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class UpdateUserHandlerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public UpdateUserHandlerTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateUser_WhenRequestIsValid()
    {
        _groupRepositoryMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Ira" };
        var group = new Group { Name = "PI-25" };

        _userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.Update(user)).ReturnsAsync(user);

        _groupRepositoryMock.Setup(x => x.GetGroupByNameAsync("PI-25")).ReturnsAsync(group);

        var handler = new UpdateUserHandler(_userRepositoryMock.Object, _groupRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new UpdateUserCommand { Id = user.Id, GroupName = "PI-25" }, CancellationToken.None);

        // Assert
        Assert.Equal(group, result.Group);
        _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateUser_WhenUserNotFound()
    {
        _groupRepositoryMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User)null!);

        var handler = new UpdateUserHandler(_userRepositoryMock.Object, _groupRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new UpdateUserCommand { Id = Guid.NewGuid(), GroupName = "PI-25" }, CancellationToken.None));
    }
}



