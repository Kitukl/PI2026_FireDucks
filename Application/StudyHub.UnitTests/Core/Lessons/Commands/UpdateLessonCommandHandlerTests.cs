using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Commands;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lessons.Commands;

public class UpdateLessonCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapAndUpdateLesson()
    {
        // Arrange
        var repositoryMock = new Mock<ILessonRepository>();
        var handler = new UpdateLessonCommandHandler(repositoryMock.Object);

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
        repositoryMock.Verify(x => x.UpdateLesson(It.Is<Lesson>(l => l.Id == dto.Id && l.LessonType == "Practice")), Times.Once);
    }
}


