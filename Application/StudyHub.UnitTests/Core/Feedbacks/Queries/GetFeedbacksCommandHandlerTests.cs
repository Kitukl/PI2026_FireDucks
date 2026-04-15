using Moq;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Feedbacks.Queries;

public class GetFeedbacksCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnList()
    {
        // Arrange
        var list = new List<Feedback>
        {
            new() { Id = Guid.NewGuid(), Description = "a", CreatorFullname = "u", User = new User() }
        };

        var repositoryMock = new Mock<IFeedbackRepository>();
        repositoryMock.Setup(x => x.GetFeedbacksAsync()).ReturnsAsync(list);

        var handler = new GetFeedbacksCommandHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetFeedbacksCommand(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        repositoryMock.Verify(x => x.GetFeedbacksAsync(), Times.Once);
    }
}
