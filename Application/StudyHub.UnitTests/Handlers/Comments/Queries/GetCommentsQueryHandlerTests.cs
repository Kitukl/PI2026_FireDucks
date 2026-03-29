using Moq;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Comments.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Comments.Queries;

public class GetCommentsQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnCommentsForTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var comments = new List<Comment>
        {
            new() { Id = Guid.NewGuid(), UserName = "A", Description = "One" },
            new() { Id = Guid.NewGuid(), UserName = "B", Description = "Two" }
        };

        var repositoryMock = new Mock<ICommentRepository>();
        repositoryMock.Setup(x => x.GetCommentsAsync(taskId)).ReturnsAsync(comments);

        var handler = new GetCommentsQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetCommentsQuery { TaskId = taskId }, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        repositoryMock.Verify(x => x.GetCommentsAsync(taskId), Times.Once);
    }
}
