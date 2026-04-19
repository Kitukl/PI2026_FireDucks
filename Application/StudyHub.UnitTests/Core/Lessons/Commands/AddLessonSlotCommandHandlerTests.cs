using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Commands;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lessons.Commands;

public class AddLessonSlotCommandHandlerTests
{
    private readonly Mock<ILessonRepository> _repositoryMock;

    public AddLessonSlotCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILessonRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new AddLessonSlotCommandHandler(_repositoryMock.Object);

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
        _repositoryMock.Verify(x => x.AddLesson(It.Is<Lesson>(l => l.Id != Guid.Empty && l.Subject.Name == "Math")), Times.Once);
    }
}

