using Moq;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Comments.Commands;

public class CreateCommentCommandHandlerTests
{
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    public CreateCommentCommandHandlerTests()
    {
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _taskRepositoryMock = new Mock<ITaskRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCreateComment_WhenRequestIsValid()
    {
        _commentRepositoryMock.Reset();
        _taskRepositoryMock.Reset();
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new StudyHub.Domain.Entities.Task { Id = taskId, Title = "T", Description = "D", Subject = new Subject { Name = "Math" }, User = new User() };
        var expectedId = Guid.NewGuid();

        _commentRepositoryMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>())).ReturnsAsync(expectedId);

        _taskRepositoryMock.Setup(x => x.GetTaskAsync(taskId)).ReturnsAsync(task);

        var handler = new CreateCommentCommandHandler(_commentRepositoryMock.Object, _taskRepositoryMock.Object);
        var command = new CreateCommentCommand { TaskId = taskId, UserName = "User", Description = "Text" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        _commentRepositoryMock.Verify(x => x.CreateCommentAsync(It.Is<Comment>(c =>
        c.Task == task &&
        c.UserName == "User" &&
        c.Description == "Text")), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCreateComment_WhenTaskNotFound()
    {
        _commentRepositoryMock.Reset();
        _taskRepositoryMock.Reset();
        // Arrange
        _taskRepositoryMock.Setup(x => x.GetTaskAsync(It.IsAny<Guid>())).ReturnsAsync((StudyHub.Domain.Entities.Task?)null);

        var handler = new CreateCommentCommandHandler(_commentRepositoryMock.Object, _taskRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new CreateCommentCommand { TaskId = Guid.NewGuid() }, CancellationToken.None));
        _commentRepositoryMock.Verify(x => x.CreateCommentAsync(It.IsAny<Comment>()), Times.Never);
    }
}



