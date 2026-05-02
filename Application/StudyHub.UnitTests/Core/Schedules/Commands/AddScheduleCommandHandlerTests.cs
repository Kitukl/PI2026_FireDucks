using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class AddScheduleCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public AddScheduleCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAddSchedule_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new AddScheduleCommandHandler(_repositoryMock.Object);

        var dto = new ScheduleDto
        {
            Id = Guid.Empty,
            Group = new GroupDto { Id = Guid.NewGuid(), Name = "PI-21" },
            LeaderUpdate = true,
            IsAutoUpdate = true,
            UpdateAt = DateTime.UtcNow,
            Lessons = new List<LessonDto>
        {
        new()
        {
        Id = Guid.Empty,
        Day = DayOfWeek.Monday,
        LessonType = "Lecture",
        Subject = new SubjectDto { Id = Guid.Empty, Name = "Math" },
        LessonSlot = new LessonSlotDto { Id = Guid.Empty, StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(9,0) },
        Lecturers = new List<LecturerDtoResponse>()
        }
        }
        };

        // Act
        await handler.Handle(new AddScheduleRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddSchedule(It.Is<Schedule>(s =>
        s.Id != Guid.Empty &&
        s.Group.Name == "PI-21" &&
        s.Lessons.Count == 1)), Times.Once);
    }
}



