using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Commands;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lessons.Commands;

public class UpdateLessonCommandHandlerTests
{
    private readonly Mock<ILessonRepository> _repositoryMock;

    public UpdateLessonCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILessonRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new UpdateLessonCommandHandler(_repositoryMock.Object);

        var dto = new LessonDto
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Friday,
            LessonType = "Practice",
            Subject = new SubjectDto { Id = Guid.NewGuid(), Name = "Physics" },
            LessonSlot = new LessonSlotDto { StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(13, 0) },
            Lecturers = new List<LecturerDtoResponse>()
        };

        // Act
        await handler.Handle(new UpdateLessonRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateLesson(It.Is<Lesson>(l => l.Id == dto.Id && l.LessonType == "Practice")), Times.Once);
    }
}

