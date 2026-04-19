using Moq;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Feedbacks.Queries;

public class GetFeedbacksCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _repositoryMock;

    public GetFeedbacksCommandHandlerTests()
    {
        _repositoryMock = new Mock<IFeedbackRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetFeedbacks_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var list = new List<Feedback>
        {
        new() { Id = Guid.NewGuid(), Description = "a", CreatorFullname = "u", User = new User() }
        };

        _repositoryMock.Setup(x => x.GetFeedbacksAsync()).ReturnsAsync(list);

        var handler = new GetFeedbacksCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetFeedbacksCommand(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        _repositoryMock.Verify(x => x.GetFeedbacksAsync(), Times.Once);
    }
}



