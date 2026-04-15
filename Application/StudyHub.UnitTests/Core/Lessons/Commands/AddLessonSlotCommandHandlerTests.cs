using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Commands;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lessons.Commands;

public class AddLessonSlotCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapAndAddLesson()
    {
        // Arrange
        var repositoryMock = new Mock<ILessonRepository>();
        var handler = new AddLessonSlotCommandHandler(repositoryMock.Object);

        var dto = new LessonDto
        {
            Id = Guid.Empty,
            Day = DayOfWeek.Monday,
            LessonType = "Lecture",
            Subject = new SubjectDto { Id = Guid.NewGuid(), Name = "Math" },
            LessonSlot = new LessonSlotDto { StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0) },
            Lecturers = new List<LecturerDtoResponse> { new() { Id = Guid.NewGuid(), Name = "L", Surname = "S" } }
        };

        // Act
        await handler.Handle(new AddLessonRequest(dto), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.AddLesson(It.Is<Lesson>(l => l.Id != Guid.Empty && l.Subject.Name == "Math")), Times.Once);
    }
}


