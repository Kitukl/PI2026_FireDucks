using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Schedules.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetLessonsByDayQueryHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public GetLessonsByDayQueryHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetLessonsByDay_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var scheduleId = Guid.NewGuid();
        var lessons = new List<Lesson>
        {
        new()
        {
        Id = Guid.Empty,
        Day = DayOfWeek.Thursday,
        LessonType = "Lecture",
        Subject = new Subject { Id = Guid.NewGuid(), Name = "Math" },
        LessonsSlot = new LessonsSlot { Id = Guid.NewGuid(), StartTime = new TimeOnly(11,0), EndTime = new TimeOnly(12,0) },
        Lecturers = new List<Lecturer>()
        }
        };

        _repositoryMock.Setup(x => x.GetLessonsByDay(scheduleId, DayOfWeek.Thursday)).ReturnsAsync(lessons);

        var handler = new GetLessonsByDayQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLessonsByDayRequest(new ScheduleDayDto { Id = scheduleId, Day = DayOfWeek.Thursday }), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.NotEqual(Guid.Empty, result[0].Id);
        Assert.Equal("Math", result[0].Subject.Name);
    }
}



