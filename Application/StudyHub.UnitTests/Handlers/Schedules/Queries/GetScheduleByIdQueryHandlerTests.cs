using Moq;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetScheduleByIdQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFound_ShouldReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var schedule = new Schedule
        {
            Id = id,
            Group = new Group { Id = Guid.NewGuid(), Name = "PI" },
            Lessons = new List<Lesson>()
        };

        var repositoryMock = new Mock<IScheduleRepository>();
        repositoryMock.Setup(x => x.GetById(id)).ReturnsAsync(schedule);

        var handler = new GetScheduleByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetScheduleByIdRequest(id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenMissing_ShouldReturnNull()
    {
        // Arrange
        var repositoryMock = new Mock<IScheduleRepository>();
        repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((Schedule?)null);

        var handler = new GetScheduleByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetScheduleByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
