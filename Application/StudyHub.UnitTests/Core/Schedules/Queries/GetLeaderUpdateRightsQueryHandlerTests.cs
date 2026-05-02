using Moq;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Schedules.Queries;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetLeaderUpdateRightsQueryHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public GetLeaderUpdateRightsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async Task Handle_ShouldGetLeaderUpdateRights_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var groupId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetLeaderUpdateRights(groupId)).ReturnsAsync(true);

        var handler = new GetLeaderUpdateRightsQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLeaderUpdateRightsRequest(groupId), CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}



