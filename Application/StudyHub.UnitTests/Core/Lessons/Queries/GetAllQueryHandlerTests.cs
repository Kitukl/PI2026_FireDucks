using Moq;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Lessons.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lessons.Queries;

public class GetAllQueryHandlerTests
{
    private readonly Mock<ILessonRepository> _repositoryMock;

    public GetAllQueryHandlerTests()
    {
        _repositoryMock = new Mock<ILessonRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var lessons = new List<Lesson>
        {
        new()
        {
        Id = Guid.NewGuid(),
        Day = DayOfWeek.Tuesday,
        LessonType = "Lecture",
        Subject = new Subject { Id = Guid.NewGuid(), Name = "Math" },
        LessonsSlot = new LessonsSlot { Id = Guid.NewGuid(), StartTime = new TimeOnly(9,0), EndTime = new TimeOnly(10,0) },
        Lecturers = new List<Lecturer> { new() { Id = Guid.NewGuid(), Name = "L", Surname = "S" } }
        }
        };

        _repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(lessons);

        var handler = new GetAllQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllLessonsRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Math", result[0].Subject.Name);
    }
}

