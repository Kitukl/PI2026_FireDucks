using Moq;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class RemoveUserFromGroupCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;

    public RemoveUserFromGroupCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Oleh", Group = new Group { Name = "PI-23" } };
        _repositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
        _repositoryMock.Setup(x => x.Update(user)).ReturnsAsync(user);

        var handler = new RemoveUserFromGroupCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new RemoveUserFromGroupCommand { UserId = user.Id }, CancellationToken.None);

        // Assert
        Assert.Equal("Oleh", result);
        Assert.Null(user.Group);
        _repositoryMock.Verify(x => x.Update(user), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_2()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetUserById(It.IsAny<Guid>())).ReturnsAsync((User)null!);
        var handler = new RemoveUserFromGroupCommandHandler(_repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new RemoveUserFromGroupCommand { UserId = Guid.NewGuid() }, CancellationToken.None));
    }
}

