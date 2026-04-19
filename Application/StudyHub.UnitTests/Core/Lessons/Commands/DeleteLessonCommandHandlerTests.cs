using Moq;
using StudyHub.Core.Lessons.Commands;
using StudyHub.Core.Lessons.Interfaces;

namespace StudyHub.UnitTests.Handlers.Lessons.Commands;

public class DeleteLessonCommandHandlerTests
{
    private readonly Mock<ILessonRepository> _repositoryMock;

    public DeleteLessonCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILessonRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        var handler = new DeleteLessonCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteLessonRequest(id), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.DeleteLesson(id), Times.Once);
    }
}

