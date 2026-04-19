using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class UpdateHeadmanRightsCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public UpdateHeadmanRightsCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateHeadmanRights_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var groupId = Guid.NewGuid();
        var schedule = new Schedule { Id = Guid.NewGuid(), Group = new Group { Id = groupId, Name = "PI" }, Lessons = new List<Lesson>() };
        _repositoryMock.Setup(x => x.GetByGroupIdAsync(groupId)).ReturnsAsync(schedule);

        var handler = new UpdateHeadmanRightsCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateHeadmanRightsRequest(new ScheduleHeadmanRightsUpdateDtoRequest { Id = groupId }), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateHeadmanRights(schedule), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateHeadmanRights_WhenScheduleNotFound()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetByGroupIdAsync(It.IsAny<Guid>())).ReturnsAsync((Schedule?)null);

        var handler = new UpdateHeadmanRightsCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateHeadmanRightsRequest(new ScheduleHeadmanRightsUpdateDtoRequest { Id = Guid.NewGuid() }), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateHeadmanRights(It.IsAny<Schedule>()), Times.Never);
    }
}



