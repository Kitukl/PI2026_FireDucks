using Moq;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Interfaces;

namespace StudyHub.UnitTests.Handlers.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;

    public DeleteTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var taskId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.DeleteTaskAsync(taskId)).ReturnsAsync(taskId);

        var handler = new DeleteTaskCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new DeleteCommand { Id = taskId }, CancellationToken.None);

        // Assert
        Assert.Equal(taskId, result);
        _repositoryMock.Verify(x => x.DeleteTaskAsync(taskId), Times.Once);
    }
}

