using Moq;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Users.Queries;

public class GetUserHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;

    public GetUserHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        var user = new User
        {
            Id = id,
            Name = "Anna",
            Surname = "Smith",
            PhotoUrl = "avatar.jpg",
            Group = new Group { Name = "PI-22" }
        };

        _repositoryMock.Setup(x => x.GetUserById(id)).ReturnsAsync(user);
        _repositoryMock.Setup(x => x.GetRolesByUser(user)).ReturnsAsync(new List<string> { "Leader" });

        var handler = new GetUserHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetUserRequest(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal("Anna", result.Name);
        Assert.Equal("Smith", result.Surname);
        Assert.Equal("PI-22", result.GroupName);
        Assert.Contains("Leader", result.Roles);
        _repositoryMock.Verify(x => x.GetUserById(id), Times.Once);
        _repositoryMock.Verify(x => x.GetRolesByUser(user), Times.Once);
    }
}

