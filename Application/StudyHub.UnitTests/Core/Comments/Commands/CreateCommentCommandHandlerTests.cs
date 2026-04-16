using Moq;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Comments.Commands;

public class CreateCommentCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTaskExists_ShouldCreateComment()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new StudyHub.Domain.Entities.Task { Id = taskId, Title = "T", Description = "D", Subject = new Subject { Name = "Math" }, User = new User() };
        var expectedId = Guid.NewGuid();

        var commentRepositoryMock = new Mock<ICommentRepository>();
        commentRepositoryMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>())).ReturnsAsync(expectedId);

        var taskRepositoryMock = new Mock<ITaskRepository>();
        taskRepositoryMock.Setup(x => x.GetTaskAsync(taskId)).ReturnsAsync(task);

        var handler = new CreateCommentCommandHandler(commentRepositoryMock.Object, taskRepositoryMock.Object);
        var command = new CreateCommentCommand { TaskId = taskId, UserName = "User", Description = "Text" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        commentRepositoryMock.Verify(x => x.CreateCommentAsync(It.Is<Comment>(c =>
            c.Task == task &&
            c.UserName == "User" &&
            c.Description == "Text")), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTaskMissing_ShouldThrow()
    {
        // Arrange
        var taskRepositoryMock = new Mock<ITaskRepository>();
        taskRepositoryMock.Setup(x => x.GetTaskAsync(It.IsAny<Guid>())).ReturnsAsync((StudyHub.Domain.Entities.Task?)null);

        var commentRepositoryMock = new Mock<ICommentRepository>();
        var handler = new CreateCommentCommandHandler(commentRepositoryMock.Object, taskRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new CreateCommentCommand { TaskId = Guid.NewGuid() }, CancellationToken.None));
        commentRepositoryMock.Verify(x => x.CreateCommentAsync(It.IsAny<Comment>()), Times.Never);
    }
}
