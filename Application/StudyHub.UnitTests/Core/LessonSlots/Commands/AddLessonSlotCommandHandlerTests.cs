using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Commands;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.LessonSlots.Commands;

public class AddLessonSlotCommandHandlerTests
{
    private readonly Mock<ILessonSlotRepository> _repositoryMock;

    public AddLessonSlotCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILessonSlotRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new AddLessonSlotCommandHandler(_repositoryMock.Object);
        var dto = new LessonSlotDto { Id = Guid.Empty, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0) };

        // Act
        await handler.Handle(new AddLessonSlotRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddLessonSlot(It.Is<LessonsSlot>(s => s.Id != Guid.Empty && s.StartTime == dto.StartTime)), Times.Once);
    }
}

