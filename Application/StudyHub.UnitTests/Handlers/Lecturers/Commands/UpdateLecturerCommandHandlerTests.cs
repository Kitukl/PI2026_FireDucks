using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Commands;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lecturers.Commands;

public class UpdateLecturerCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapAndUpdateLecturer()
    {
        // Arrange
        var repositoryMock = new Mock<ILecturerRepository>();
        var handler = new UpdateLecturerCommandHandler(repositoryMock.Object);

        var lecturerId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        var dto = new LecturerDtoRequest
        {
            Id = lecturerId,
            Name = "Ira",
            Surname = "S",
            Lessons = new List<LessonDto>
            {
                new()
                {
                    Id = lessonId,
                    Day = DayOfWeek.Tuesday,
                    LessonType = "Practice",
                    Subject = new SubjectDto { Id = Guid.NewGuid(), Name = "Physics" },
                    LessonSlot = new LessonSlotDto { Id = Guid.NewGuid(), StartTime = new TimeOnly(10,0), EndTime = new TimeOnly(11,0) },
                    Lecturers = new List<LecturerDtoResponse>()
                }
            }
        };

        // Act
        await handler.Handle(new UpdateLecturerRequest(dto), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.UpdateLecturer(It.Is<Lecturer>(l =>
            l.Id == lecturerId &&
            l.Lessons.First().Id == lessonId)), Times.Once);
    }
}


