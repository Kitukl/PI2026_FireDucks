using MediatR;
using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Core.Users.Queries;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Tasks.Commands;

public class EditTaskBoardTaskCommandTests
{
    private readonly Mock<ISender> _senderMock;

    public EditTaskBoardTaskCommandTests()
    {
        _senderMock = new Mock<ISender>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnForbidden_WhenUserIdIsNull()
    {
        // Arrange
        var handler = new EditTaskBoardTaskCommandHandler(_senderMock.Object);
        var command = new EditTaskBoardTaskCommand
        {
            CurrentUserId = null,
            TaskId = Guid.NewGuid(),
            Title = "New Title",
            ResourceUrl = "https://example.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsForbidden);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var task = new StudyHub.Domain.Entities.Task
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Desc",
            Status = Status.ToDo,
            Subject = new Subject { Name = "Math" },
            User = new User { Id = userId }
        };

        var userDto = new UserDto
        {
            Id = userId,
            Name = "Test",
            Surname = "User",
            GroupName = "ПМІ-31",
            Roles = new List<string>()
        };

        _senderMock
            .Setup(x => x.Send(It.IsAny<GetTaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _senderMock
            .Setup(x => x.Send(It.IsAny<GetUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDto);

        _senderMock
            .Setup(x => x.Send(It.IsAny<IsTaskVisibleForUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _senderMock
            .Setup(x => x.Send(It.IsAny<UpdateTaskFullCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskId);

        var handler = new EditTaskBoardTaskCommandHandler(_senderMock.Object);

        var command = new EditTaskBoardTaskCommand
        {
            CurrentUserId = userId,
            TaskId = taskId,
            Title = "New Title",
            Description = "New Desc",
            ResourceUrl = "https://example.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnForbidden_WhenTaskNotVisible()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var task = new StudyHub.Domain.Entities.Task
        {
            Id = taskId,
            Title = "Task",
            Status = Status.ToDo,
            Subject = new Subject { Name = "Math" },
            User = new User { Id = Guid.NewGuid() }
        };

        var userDto = new UserDto
        {
            Id = userId,
            Name = "Test",
            Surname = "User",
            GroupName = "ПМІ-31",
            Roles = new List<string>()
        };

        _senderMock
            .Setup(x => x.Send(It.IsAny<GetTaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _senderMock
            .Setup(x => x.Send(It.IsAny<GetUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDto);

        _senderMock
            .Setup(x => x.Send(It.IsAny<IsTaskVisibleForUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new EditTaskBoardTaskCommandHandler(_senderMock.Object);

        var command = new EditTaskBoardTaskCommand
        {
            CurrentUserId = userId,
            TaskId = taskId,
            Title = "New Title",
            ResourceUrl = "https://example.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsForbidden);
    }
}