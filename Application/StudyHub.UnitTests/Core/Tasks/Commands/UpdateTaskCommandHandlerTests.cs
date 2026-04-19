using Moq;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Tasks.Commands;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;

    public UpdateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateTask_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var taskId = Guid.NewGuid();
        var originalTask = new StudyHub.Domain.Entities.Task
        {
            Id = taskId,
            Title = "Task",
            Description = "Desc",
            Status = Status.ToDo,
            Subject = new Subject { Name = "Old" },
            User = new User()
        };

        var newSubject = new Subject { Name = "New" };

        _repositoryMock.Setup(x => x.GetTaskAsync(taskId)).ReturnsAsync(originalTask);
        _repositoryMock.Setup(x => x.UpdateTaskAsync(originalTask)).ReturnsAsync(taskId);

        var handler = new UpdateTaskCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new UpdateTaskCommand
        {
            Id = taskId,
            Status = Status.Done,
            Subject = newSubject
        }, CancellationToken.None);

        // Assert
        Assert.Equal(taskId, result);
        Assert.Equal(Status.Done, originalTask.Status);
        Assert.Equal(newSubject, originalTask.Subject);
        _repositoryMock.Verify(x => x.UpdateTaskAsync(originalTask), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateTask_WhenTaskNotFound()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetTaskAsync(It.IsAny<Guid>())).ReturnsAsync((StudyHub.Domain.Entities.Task?)null);

        var handler = new UpdateTaskCommandHandler(_repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Status = Status.InProgress,
            Subject = new Subject { Name = "Math" }
        }, CancellationToken.None));
    }
}



