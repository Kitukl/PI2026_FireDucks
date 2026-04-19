using Moq;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Queries;

public class GetUsersHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;

    public GetUsersHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetUsers_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "John",
            Surname = "Doe",
            PhotoUrl = "photo.png",
            Group = new Group { Name = "PI-21" }
        };

        _repositoryMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(new[] { user });
        _repositoryMock.Setup(x => x.GetRolesByUser(user)).ReturnsAsync(new List<string> { "Student" });

        var handler = new GetUsersHandler(_repositoryMock.Object);

        // Act
        var result = (await handler.Handle(new GetUsersRequest(), CancellationToken.None)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(user.Id, result[0].Id);
        Assert.Equal("John", result[0].Name);
        Assert.Equal("Doe", result[0].Surname);
        Assert.Equal("PI-21", result[0].GroupName);
        Assert.Contains("Student", result[0].Roles);
        _repositoryMock.Verify(x => x.GetUsersAsync(), Times.Once);
        _repositoryMock.Verify(x => x.GetRolesByUser(user), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetUsers_WhenNoUsersExist()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(Array.Empty<User>());

        var handler = new GetUsersHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetUsersRequest(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
        _repositoryMock.Verify(x => x.GetUsersAsync(), Times.Once);
    }
}



