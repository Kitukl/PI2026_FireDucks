using Moq;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Tasks.Commands;

public class CreateTaskCommandsHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCreateTaskWithTrimmedDescriptionAndDefaultStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var subject = new Subject { Id = Guid.NewGuid(), Name = "Math" };
        var user = new User { Id = userId, Name = "User" };
        var deadline = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Unspecified);
        var expectedId = Guid.NewGuid();

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);

        var taskRepositoryMock = new Mock<ITaskRepository>();
        taskRepositoryMock
            .Setup(x => x.AddTaskAsync(It.IsAny<StudyHub.Domain.Entities.Task>()))
            .ReturnsAsync(expectedId);

        var handler = new CreateTaskCommandsHandler(taskRepositoryMock.Object, userRepositoryMock.Object);

        var command = new CreateTaskCommand
        {
            UserId = userId,
            Subject = subject,
            Title = "Task title",
            Description = "  some desc  ",
            Deadline = deadline,
            IsGroupTask = true
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        taskRepositoryMock.Verify(x => x.AddTaskAsync(It.Is<StudyHub.Domain.Entities.Task>(task =>
            task.User == user &&
            task.Subject == subject &&
            task.Title == "Task title" &&
            task.Description == "some desc" &&
            task.Status == Status.ToDo &&
            task.IsGroupTask &&
            task.Deadline.Kind == DateTimeKind.Utc)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenDeadlineIsLocal_ShouldConvertToUtc()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var localDeadline = new DateTime(2026, 4, 2, 15, 30, 0, DateTimeKind.Local);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);

        var taskRepositoryMock = new Mock<ITaskRepository>();
        taskRepositoryMock
            .Setup(x => x.AddTaskAsync(It.IsAny<StudyHub.Domain.Entities.Task>()))
            .ReturnsAsync(Guid.NewGuid());

        var handler = new CreateTaskCommandsHandler(taskRepositoryMock.Object, userRepositoryMock.Object);

        // Act
        await handler.Handle(new CreateTaskCommand
        {
            UserId = userId,
            Title = "Task",
            Description = "x",
            Subject = new Subject { Name = "Physics" },
            Deadline = localDeadline
        }, CancellationToken.None);

        // Assert
        taskRepositoryMock.Verify(x => x.AddTaskAsync(It.Is<StudyHub.Domain.Entities.Task>(task => task.Deadline.Kind == DateTimeKind.Utc)), Times.Once);
    }
}


