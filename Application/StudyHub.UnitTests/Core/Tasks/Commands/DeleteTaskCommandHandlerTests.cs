using Moq;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Interfaces;

namespace StudyHub.UnitTests.Handlers.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCallDeleteAndReturnId()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.DeleteTaskAsync(taskId)).ReturnsAsync(taskId);

        var handler = new DeleteTaskCommandHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new DeleteCommand { Id = taskId }, CancellationToken.None);

        // Assert
        Assert.Equal(taskId, result);
        repositoryMock.Verify(x => x.DeleteTaskAsync(taskId), Times.Once);
    }
}


