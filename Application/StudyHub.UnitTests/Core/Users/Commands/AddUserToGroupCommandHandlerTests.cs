using Moq;
using StudyHub.Core.Group;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class AddUserToGroupCommandHandlerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public AddUserToGroupCommandHandlerTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAddUserToGroup_WhenRequestIsValid()
    {
        _groupRepositoryMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Nazar" };
        var group = new Group { Id = Guid.NewGuid(), Name = "PI-24" };

        _userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.Update(user)).ReturnsAsync(user);

        _groupRepositoryMock.Setup(x => x.GetGroupByNameAsync("PI-24")).ReturnsAsync(group);

        var handler = new AddUserToGroupCommandHandler(_userRepositoryMock.Object, _groupRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new AddUserToGroupCommand { UserId = user.Id, GroupName = "PI-24" }, CancellationToken.None);

        // Assert
        Assert.Equal("PI-24", result);
        Assert.Equal(group, user.Group);
        _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }
}



