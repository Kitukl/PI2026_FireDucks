using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Commands;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lecturers.Commands;

public class AddLecturerCommandHandlerTests
{
    private readonly Mock<ILecturerRepository> _repositoryMock;

    public AddLecturerCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILecturerRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAddLecturer_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new AddLecturerCommandHandler(_repositoryMock.Object);

        var dto = new LecturerDtoRequest
        {
            Id = Guid.Empty,
            Name = "Ivan",
            Surname = "Petrov",
            Lessons = new List<LessonDto>
        {
        new()
        {
        Id = Guid.Empty,
        Day = DayOfWeek.Monday,
        LessonType = "Lecture",
        Subject = new SubjectDto { Id = Guid.NewGuid(), Name = "Math" },
        LessonSlot = new LessonSlotDto { StartTime = new TimeOnly(9,0), EndTime = new TimeOnly(10,0) },
        Lecturers = new List<LecturerDtoResponse> { new() { Id = Guid.NewGuid(), Name = "A", Surname = "B" } }
        }
        }
        };

        // Act
        await handler.Handle(new AddLecturerRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddLecturer(It.Is<Lecturer>(l =>
        l.Name == "Ivan" &&
        l.Surname == "Petrov" &&
        l.Id != Guid.Empty &&
        l.Lessons.Count == 1)), Times.Once);
    }
}



