using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class SetScheduleAutoUpdateIntervalCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public SetScheduleAutoUpdateIntervalCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new SetScheduleAutoUpdateIntervalCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new SetScheduleAutoUpdateIntervalRequest(15), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.SetScheduleAutoUpdateInterval(15), Times.Once);
    }
}

