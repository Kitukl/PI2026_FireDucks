using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class SetScheduleAutoUpdateCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldSetAutoUpdateFlag()
    {
        // Arrange
        var repositoryMock = new Mock<IScheduleRepository>();
        var handler = new SetScheduleAutoUpdateCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new SetScheduleAutoUpdateRequest(true), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.SetScheduleAutoUpdate(true), Times.Once);
    }
}
