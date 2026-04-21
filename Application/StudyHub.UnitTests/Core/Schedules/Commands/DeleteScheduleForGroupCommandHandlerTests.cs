using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class DeleteScheduleForGroupCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public DeleteScheduleForGroupCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteScheduleForGroup_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var groupId = Guid.NewGuid();
        var handler = new DeleteScheduleForGroupCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteScheduleForGroupRequest(groupId), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.DeleteScheduleForGroup(groupId), Times.Once);
    }
}



