using Moq;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Tasks.Queries;

public class GetGroupedTaskStatsHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;

    public GetGroupedTaskStatsHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetGroupedTaskStats_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var source = new Dictionary<bool, Dictionary<Status, int>>
        {
            [false] = new Dictionary<Status, int> { [Status.ToDo] = 2 },
            [true] = new Dictionary<Status, int> { [Status.Done] = 3 }
        };

        _repositoryMock.Setup(x => x.GetGroupedTaskStatsAsync()).ReturnsAsync(source);

        var handler = new GetGroupedTaskStatsHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetGroupedTaskStatsRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result["User Tasks"]["ToDo"]);
        Assert.Equal(3, result["Group Tasks"]["Done"]);
    }
}



