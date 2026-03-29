using Moq;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Statistics.Queries;

public class GetUsersStatisticHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenRecentStatisticMissing_ShouldReturnEmptyActivityAndCurrentFileCount()
    {
        // Arrange
        var repositoryMock = new Mock<IStatisticRepository>();
        repositoryMock.Setup(x => x.GetRecentStatisticAsync()).ReturnsAsync((Statistic?)null);
        repositoryMock.Setup(x => x.GetStorageFileCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(9);

        var handler = new GetUsersStatisticHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetUsersStatisticRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(9, result.FileCount);
        Assert.Empty(result.UserActivityPerMonth);
        repositoryMock.Verify(x => x.GetYearlyActivityAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenRecentStatisticExists_ShouldReturnMappedDto()
    {
        // Arrange
        var statistic = new Statistic { CreatedAt = new DateTime(2026, 1, 1), FilesCount = 1 };
        var yearly = new Dictionary<int, double> { [1] = 0.5 };

        var repositoryMock = new Mock<IStatisticRepository>();
        repositoryMock.Setup(x => x.GetRecentStatisticAsync()).ReturnsAsync(statistic);
        repositoryMock.Setup(x => x.GetYearlyActivityAsync(It.IsAny<int>())).ReturnsAsync(yearly);
        repositoryMock.Setup(x => x.GetStorageFileCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(13);

        var handler = new GetUsersStatisticHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetUsersStatisticRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(statistic.CreatedAt, result.CreatedAt);
        Assert.Equal(13, result.FileCount);
        Assert.Equal(0.5, result.UserActivityPerMonth[1]);
    }
}
