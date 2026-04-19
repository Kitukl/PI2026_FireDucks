using Moq;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetAllSchedulesQueryHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public GetAllSchedulesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var schedules = new List<Schedule>
        {
        new()
        {
        Id = Guid.NewGuid(),
        Group = new Group { Id = Guid.NewGuid(), Name = "PI-21" },
        Lessons = new List<Lesson>
        {
        new()
        {
        Id = Guid.NewGuid(),
        Day = DayOfWeek.Monday,
        LessonType = "Lecture",
        Subject = new Subject { Id = Guid.NewGuid(), Name = "Math" },
        LessonsSlot = new LessonsSlot { Id = Guid.NewGuid(), StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(9,0) },
        Lecturers = new List<Lecturer>()
        }
        }
        }
        };

        _repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(schedules);

        var handler = new GetAllSchedulesQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllSchedulesRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("PI-21", result[0].Group.Name);
    }
}

