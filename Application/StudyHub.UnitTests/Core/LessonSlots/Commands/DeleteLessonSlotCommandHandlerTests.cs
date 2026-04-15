using Moq;
using StudyHub.Core.LessonSlots.Commands;
using StudyHub.Core.LessonSlots.Interfaces;

namespace StudyHub.UnitTests.Handlers.LessonSlots.Commands;

public class DeleteLessonSlotCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteLessonSlot()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<ILessonSlotRepository>();
        var handler = new DeleteLessonSlotCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteLessonSlotRequest(id), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.DeleteLessonSlot(id), Times.Once);
    }
}
