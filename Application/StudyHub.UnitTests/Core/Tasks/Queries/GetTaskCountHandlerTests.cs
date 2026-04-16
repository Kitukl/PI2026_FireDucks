using Moq;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Tasks.Queries;

namespace StudyHub.UnitTests.Handlers.Tasks.Queries;

public class GetTaskCountHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnCountFromRepository()
    {
        // Arrange
        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.GetCountAsync()).ReturnsAsync(7);

        var handler = new GetTaskCountHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetTaskCountRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(7, result);
        repositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
    }
}


