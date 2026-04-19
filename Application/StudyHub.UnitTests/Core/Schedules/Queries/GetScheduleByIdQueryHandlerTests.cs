using Moq;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetScheduleByIdQueryHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public GetScheduleByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetScheduleById_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        var schedule = new Schedule
        {
            Id = id,
            Group = new Group { Id = Guid.NewGuid(), Name = "PI" },
            Lessons = new List<Lesson>()
        };

        _repositoryMock.Setup(x => x.GetById(id)).ReturnsAsync(schedule);

        var handler = new GetScheduleByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetScheduleByIdRequest(id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetScheduleById_WhenScheduleNotFound()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((Schedule?)null);

        var handler = new GetScheduleByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetScheduleByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}



