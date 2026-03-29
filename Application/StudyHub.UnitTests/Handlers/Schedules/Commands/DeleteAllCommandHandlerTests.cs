using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class DeleteAllCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteAllSchedules()
    {
        // Arrange
        var repositoryMock = new Mock<IScheduleRepository>();
        var handler = new DeleteAllCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteAllRequest(), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.DeleteAllAsync(), Times.Once);
    }
}
