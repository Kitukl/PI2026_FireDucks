using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class DeleteScheduleForGroupCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteScheduleForGroup()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var repositoryMock = new Mock<IScheduleRepository>();
        var handler = new DeleteScheduleForGroupCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteScheduleForGroupRequest(groupId), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.DeleteScheduleForGroup(groupId), Times.Once);
    }
}
