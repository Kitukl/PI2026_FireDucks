using System.Security.Claims;
using Application.Helpers;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Common.Helpers;

public class TaskBoardControllerHelperTests
{
    [Fact]
    public void GetCurrentUserId_ShouldReturnGuid_WhenClaimContainsValidGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        });
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = TaskBoardControllerHelper.GetCurrentUserId(user);

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public void ParseTaskStatus_ShouldReturnInProgress_WhenInputIsInProgressWithSpaces()
    {
        // Arrange
        const string rawStatus = "in progress";

        // Act
        var result = TaskBoardControllerHelper.ParseTaskStatus(rawStatus);

        // Assert
        Assert.Equal(Status.InProgress, result);
    }

    [Fact]
    public void NormalizeTaskStatus_ShouldReturnDone_WhenStatusIsResolved()
    {
        // Arrange
        const Status status = Status.Resolved;

        // Act
        var result = TaskBoardControllerHelper.NormalizeTaskStatus(status);

        // Assert
        Assert.Equal("done", result);
    }
}
