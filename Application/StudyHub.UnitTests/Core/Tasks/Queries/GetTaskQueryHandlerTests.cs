using Moq;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Tasks.Queries;

public class GetTaskQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;

    public GetTaskQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        var task = new StudyHub.Domain.Entities.Task { Id = id, Title = "Task", Description = "Desc", Subject = new Subject { Name = "Math" }, User = new User() };

        _repositoryMock.Setup(x => x.GetTaskAsync(id)).ReturnsAsync(task);

        var handler = new GetTaskQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetTaskQuery { Id = id }, CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Id);
        _repositoryMock.Verify(x => x.GetTaskAsync(id), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_2()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetTaskAsync(It.IsAny<Guid>())).ReturnsAsync((StudyHub.Domain.Entities.Task?)null);

        var handler = new GetTaskQueryHandler(_repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new GetTaskQuery { Id = Guid.NewGuid() }, CancellationToken.None));
    }
}

