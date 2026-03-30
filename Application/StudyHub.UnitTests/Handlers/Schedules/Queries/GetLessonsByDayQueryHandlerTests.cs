using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Schedules.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetLessonsByDayQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnMappedLessons()
    {
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

        var repositoryMock = new Mock<IScheduleRepository>();
        repositoryMock.Setup(x => x.GetLessonsByDay(scheduleId, DayOfWeek.Thursday)).ReturnsAsync(lessons);

        var handler = new GetLessonsByDayQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLessonsByDayRequest(new ScheduleDayDto { Id = scheduleId, Day = DayOfWeek.Thursday }), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.NotEqual(Guid.Empty, result[0].Id);
        Assert.Equal("Math", result[0].Subject.Name);
    }
}


