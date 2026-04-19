using Moq;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Comments.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Comments.Queries;

public class GetCommentsQueryHandlerTests
{
    private readonly Mock<ICommentRepository> _repositoryMock;

    public GetCommentsQueryHandlerTests()
    {
        _repositoryMock = new Mock<ICommentRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetComments_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var taskId = Guid.NewGuid();
        var comments = new List<Comment>
        {
        new() { Id = Guid.NewGuid(), UserName = "A", Description = "One" },
        new() { Id = Guid.NewGuid(), UserName = "B", Description = "Two" }
        };

        _repositoryMock.Setup(x => x.GetCommentsAsync(taskId)).ReturnsAsync(comments);

        var handler = new GetCommentsQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetCommentsQuery { TaskId = taskId }, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(x => x.GetCommentsAsync(taskId), Times.Once);
    }
}



