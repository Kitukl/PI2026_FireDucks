using Moq;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Schedules.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetScheduleByGroupIdQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFound_ShouldReturnDto()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            Group = new Group { Id = groupId, Name = "PI-31" },
            Lessons = new List<Lesson>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Day = DayOfWeek.Monday,
                    LessonType = "Lec",
                    Subject = new Subject { Id = Guid.NewGuid(), Name = "Math" },
                    LessonsSlot = new LessonsSlot { Id = Guid.NewGuid(), StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(9,0) },
                    Lecturers = new List<Lecturer> { new() { Id = Guid.Empty, Name = "A", Surname = "B" } }
                }
            }
        };

        var repositoryMock = new Mock<IScheduleRepository>();
        repositoryMock.Setup(x => x.GetByGroupIdAsync(groupId)).ReturnsAsync(schedule);

        var handler = new GetScheduleByGroupIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetScheduleByGroupIdRequest(groupId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PI-31", result!.Group.Name);
        Assert.NotEqual(Guid.Empty, result.Lessons[0].Lecturers![0].Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenMissing_ShouldReturnNull()
    {
        // Arrange
        var repositoryMock = new Mock<IScheduleRepository>();
        repositoryMock.Setup(x => x.GetByGroupIdAsync(It.IsAny<Guid>())).ReturnsAsync((Schedule?)null);

        var handler = new GetScheduleByGroupIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetScheduleByGroupIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}


