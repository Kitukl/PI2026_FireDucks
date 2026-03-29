using Moq;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Tasks.Commands;

public class UpdateTaskCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTaskExists_ShouldUpdateStatusAndSubject()
    {
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

        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.GetTaskAsync(taskId)).ReturnsAsync(originalTask);
        repositoryMock.Setup(x => x.UpdateTaskAsync(originalTask)).ReturnsAsync(taskId);

        var handler = new UpdateTaskCommandHandler(repositoryMock.Object);

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
        repositoryMock.Verify(x => x.UpdateTaskAsync(originalTask), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTaskMissing_ShouldThrow()
    {
        // Arrange
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.GetTaskAsync(It.IsAny<Guid>())).ReturnsAsync((StudyHub.Domain.Entities.Task?)null);

        var handler = new UpdateTaskCommandHandler(repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new UpdateTaskCommand
        {
            Id = Guid.NewGuid(),
            Status = Status.InProgress,
            Subject = new Subject { Name = "Math" }
        }, CancellationToken.None));
    }
}


