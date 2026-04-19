using Moq;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Interfaces;

namespace StudyHub.UnitTests.Handlers.Comments.Commands;

public class DeleteCommentCommandHandlerTests
{
    private readonly Mock<ICommentRepository> _repositoryMock;

    public DeleteCommentCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICommentRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteComment_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(x => x.DeleteCommentAsync(id)).ReturnsAsync(id);

        var handler = new DeleteCommentCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new DeleteCommentCommand { Id = id }, CancellationToken.None);

        // Assert
        Assert.Equal(id, result);
        _repositoryMock.Verify(x => x.DeleteCommentAsync(id), Times.Once);
    }
}



