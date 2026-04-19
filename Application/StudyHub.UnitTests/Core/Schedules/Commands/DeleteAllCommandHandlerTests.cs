using Moq;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class DeleteAllCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public DeleteAllCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new DeleteAllCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteAllRequest(), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.DeleteAllAsync(), Times.Once);
    }
}

