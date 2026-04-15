using Moq;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Tasks.Queries;

public class GetTaskQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTaskExists_ShouldReturnTask()
    {
        // Arrange
        var id = Guid.NewGuid();
        var task = new StudyHub.Domain.Entities.Task { Id = id, Title = "Task", Description = "Desc", Subject = new Subject { Name = "Math" }, User = new User() };

        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.GetTaskAsync(id)).ReturnsAsync(task);

        var handler = new GetTaskQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetTaskQuery { Id = id }, CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Id);
        repositoryMock.Verify(x => x.GetTaskAsync(id), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTaskMissing_ShouldThrow()
    {
        // Arrange
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.GetTaskAsync(It.IsAny<Guid>())).ReturnsAsync((StudyHub.Domain.Entities.Task?)null);

        var handler = new GetTaskQueryHandler(repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new GetTaskQuery { Id = Guid.NewGuid() }, CancellationToken.None));
    }
}
