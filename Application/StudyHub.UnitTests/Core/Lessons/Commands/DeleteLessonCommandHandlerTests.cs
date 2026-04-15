using Moq;
using StudyHub.Core.Lessons.Commands;
using StudyHub.Core.Lessons.Interfaces;

namespace StudyHub.UnitTests.Handlers.Lessons.Commands;

public class DeleteLessonCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteLesson()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<ILessonRepository>();
        var handler = new DeleteLessonCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteLessonRequest(id), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.DeleteLesson(id), Times.Once);
    }
}
