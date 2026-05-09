using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class UpdateLeaderRightsCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public UpdateLeaderRightsCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateLeaderRights_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var groupId = Guid.NewGuid();
        var schedule = new Schedule { Id = Guid.NewGuid(), Group = new Group { Id = groupId, Name = "PI" }, Lessons = new List<Lesson>() };
        _repositoryMock.Setup(x => x.GetByGroupIdAsync(groupId)).ReturnsAsync(schedule);

        var handler = new UpdateLeaderRightsCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateLeaderRightsRequest(new ScheduleLeaderRightsUpdateDtoRequest { Id = groupId }), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateLeaderRights(schedule), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateLeaderRights_WhenScheduleNotFound()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetByGroupIdAsync(It.IsAny<Guid>())).ReturnsAsync((Schedule?)null);

        var handler = new UpdateLeaderRightsCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateLeaderRightsRequest(new ScheduleLeaderRightsUpdateDtoRequest { Id = Guid.NewGuid() }), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateLeaderRights(It.IsAny<Schedule>()), Times.Never);
    }
}



