using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Commands;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.LessonSlots.Commands;

public class UpdateLessonSlotCommandHandlerTests
{
    private readonly Mock<ILessonSlotRepository> _repositoryMock;

    public UpdateLessonSlotCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILessonSlotRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new UpdateLessonSlotCommandHandler(_repositoryMock.Object);
        var dto = new LessonSlotDto { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0) };

        // Act
        await handler.Handle(new UpdateLessonSlotRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateLessonSlot(It.Is<LessonsSlot>(s => s.Id == dto.Id && s.EndTime == dto.EndTime)), Times.Once);
    }
}

