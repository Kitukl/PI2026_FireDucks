using Moq;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Tasks.Queries;

public class GetTasksQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnTasksFromRepository()
    {
        // Arrange
        var tasks = new List<StudyHub.Domain.Entities.Task>
        {
            new() { Id = Guid.NewGuid(), Title = "T1", Description = "D1", Subject = new Subject { Name = "Math" }, User = new User() },
            new() { Id = Guid.NewGuid(), Title = "T2", Description = "D2", Subject = new Subject { Name = "Physics" }, User = new User() }
        };

        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.GetTasksAsync()).ReturnsAsync(tasks);

        var handler = new GetTasksQueryHandler(repositoryMock.Object);

        // Act
        var result = (await handler.Handle(new GetTasksQuery(), CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("T1", result[0].Title);
        repositoryMock.Verify(x => x.GetTasksAsync(), Times.Once);
    }
}

