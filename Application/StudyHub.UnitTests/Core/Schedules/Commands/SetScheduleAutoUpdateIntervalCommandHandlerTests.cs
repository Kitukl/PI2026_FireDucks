using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class SetScheduleAutoUpdateIntervalCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldSetAutoUpdateInterval()
    {
        // Arrange
        var repositoryMock = new Mock<IScheduleRepository>();
        var handler = new SetScheduleAutoUpdateIntervalCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new SetScheduleAutoUpdateIntervalRequest(15), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.SetScheduleAutoUpdateInterval(15), Times.Once);
    }
}
