using Moq;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Lessons.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lessons.Queries;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<ILessonRepository> _repositoryMock;

    public GetByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ILessonRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
            Day = DayOfWeek.Wednesday,
            LessonType = "Lecture",
            Subject = new Subject { Id = Guid.NewGuid(), Name = "Bio" },
            LessonsSlot = new LessonsSlot { Id = Guid.NewGuid(), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0) },
            Lecturers = new List<Lecturer>()
        };

        _repositoryMock.Setup(x => x.GetById(lesson.Id)).ReturnsAsync(lesson);

        var handler = new GetByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLessonByIdRequest(lesson.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Bio", result!.Subject.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_2()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((Lesson?)null);

        var handler = new GetByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLessonByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}

