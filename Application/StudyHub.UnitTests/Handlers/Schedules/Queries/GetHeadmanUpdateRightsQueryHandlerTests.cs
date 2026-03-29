using Moq;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Schedules.Queries;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetHeadmanUpdateRightsQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnRepositoryValue()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var repositoryMock = new Mock<IScheduleRepository>();
        repositoryMock.Setup(x => x.GetHeadmanUpdateRights(groupId)).ReturnsAsync(true);

        var handler = new GetHeadmanUpdateRightsQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetHeadmanUpdateRightsRequest(groupId), CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}
