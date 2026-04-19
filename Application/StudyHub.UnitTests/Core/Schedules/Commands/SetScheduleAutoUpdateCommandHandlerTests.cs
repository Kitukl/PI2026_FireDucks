using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class SetScheduleAutoUpdateCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public SetScheduleAutoUpdateCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldSetScheduleAutoUpdate_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new SetScheduleAutoUpdateCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new SetScheduleAutoUpdateRequest(true), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.SetScheduleAutoUpdate(true), Times.Once);
    }
}



