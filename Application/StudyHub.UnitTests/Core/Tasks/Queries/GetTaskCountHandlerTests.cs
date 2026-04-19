using Moq;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Tasks.Queries;

namespace StudyHub.UnitTests.Handlers.Tasks.Queries;

public class GetTaskCountHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;

    public GetTaskCountHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetCountAsync()).ReturnsAsync(7);

        var handler = new GetTaskCountHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetTaskCountRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(7, result);
        _repositoryMock.Verify(x => x.GetCountAsync(), Times.Once);
    }
}

