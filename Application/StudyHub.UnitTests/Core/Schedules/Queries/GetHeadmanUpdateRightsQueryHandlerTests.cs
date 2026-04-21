using Moq;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Schedules.Queries;

namespace StudyHub.UnitTests.Handlers.Schedules.Queries;

public class GetHeadmanUpdateRightsQueryHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public GetHeadmanUpdateRightsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetHeadmanUpdateRights_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var groupId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetHeadmanUpdateRights(groupId)).ReturnsAsync(true);

        var handler = new GetHeadmanUpdateRightsQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetHeadmanUpdateRightsRequest(groupId), CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}



