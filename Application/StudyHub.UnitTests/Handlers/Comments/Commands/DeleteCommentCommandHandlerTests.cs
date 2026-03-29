using Moq;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Interfaces;

namespace StudyHub.UnitTests.Handlers.Comments.Commands;

public class DeleteCommentCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteAndReturnId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<ICommentRepository>();
        repositoryMock.Setup(x => x.DeleteCommentAsync(id)).ReturnsAsync(id);

        var handler = new DeleteCommentCommandHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new DeleteCommentCommand { Id = id }, CancellationToken.None);

        // Assert
        Assert.Equal(id, result);
        repositoryMock.Verify(x => x.DeleteCommentAsync(id), Times.Once);
    }
}
