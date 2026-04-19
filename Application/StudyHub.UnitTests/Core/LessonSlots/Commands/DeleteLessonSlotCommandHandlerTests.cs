using Moq;
using StudyHub.Core.LessonSlots.Commands;
using StudyHub.Core.LessonSlots.Interfaces;

namespace StudyHub.UnitTests.Handlers.LessonSlots.Commands;

public class DeleteLessonSlotCommandHandlerTests
{
    private readonly Mock<ILessonSlotRepository> _repositoryMock;

    public DeleteLessonSlotCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILessonSlotRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteLessonSlot_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        var handler = new DeleteLessonSlotCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteLessonSlotRequest(id), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.DeleteLessonSlot(id), Times.Once);
    }
}



