using Moq;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Tasks.Queries;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Tasks.Queries;

public class GetGroupedTaskStatsHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapBooleanKeysToReadableGroups()
    {
        // Arrange
        var source = new Dictionary<bool, Dictionary<Status, int>>
        {
            [false] = new Dictionary<Status, int> { [Status.ToDo] = 2 },
            [true] = new Dictionary<Status, int> { [Status.Done] = 3 }
        };

        var repositoryMock = new Mock<ITaskRepository>();
        repositoryMock.Setup(x => x.GetGroupedTaskStatsAsync()).ReturnsAsync(source);

        var handler = new GetGroupedTaskStatsHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetGroupedTaskStatsRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result["User Tasks"]["ToDo"]);
        Assert.Equal(3, result["Group Tasks"]["Done"]);
    }
}


