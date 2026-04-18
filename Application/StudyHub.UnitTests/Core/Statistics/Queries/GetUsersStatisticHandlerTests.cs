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
        repositoryMock.Setup(x => x.GetStorageFileCountsAsync(It.IsAny<CancellationToken>())).ReturnsAsync((5, 4));
        repositoryMock.Setup(x => x.GetSystemEntityCountsAsync(It.IsAny<CancellationToken>())).ReturnsAsync((12, 3, 2));

        var handler = new GetUsersStatisticHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetUsersStatisticRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(5, result.UserFilesCount);
        Assert.Equal(4, result.GroupFilesCount);
        Assert.Equal(9, result.FileCount);
        Assert.Equal(12, result.StudentsCount);
        Assert.Equal(3, result.GroupsCount);
        Assert.Equal(2, result.LeadersCount);
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
        repositoryMock.Setup(x => x.GetStorageFileCountsAsync(It.IsAny<CancellationToken>())).ReturnsAsync((10, 3));
        repositoryMock.Setup(x => x.GetSystemEntityCountsAsync(It.IsAny<CancellationToken>())).ReturnsAsync((25, 7, 5));

        var handler = new GetUsersStatisticHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetUsersStatisticRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(statistic.CreatedAt, result.CreatedAt);
        Assert.Equal(10, result.UserFilesCount);
        Assert.Equal(3, result.GroupFilesCount);
        Assert.Equal(13, result.FileCount);
        Assert.Equal(25, result.StudentsCount);
        Assert.Equal(7, result.GroupsCount);
        Assert.Equal(5, result.LeadersCount);
        Assert.Equal(0.5, result.UserActivityPerMonth[1]);
    }
}
