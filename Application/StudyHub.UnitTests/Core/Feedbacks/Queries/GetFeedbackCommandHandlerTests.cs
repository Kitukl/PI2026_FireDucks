using Moq;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Feedbacks.Queries;

public class GetFeedbackCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenExists_ShouldReturnFeedback()
    {
        // Arrange
        var feedback = new Feedback { Id = Guid.NewGuid(), Description = "desc", CreatorFullname = "name", User = new User() };
        var repositoryMock = new Mock<IFeedbackRepository>();
        repositoryMock.Setup(x => x.GetFeedbackAsync(feedback.Id)).ReturnsAsync(feedback);

        var handler = new GetFeedbackCommandHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetFeedbackCommand { Id = feedback.Id }, CancellationToken.None);

        // Assert
        Assert.Equal(feedback.Id, result.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenMissing_ShouldThrow()
    {
        // Arrange
        var repositoryMock = new Mock<IFeedbackRepository>();
        repositoryMock.Setup(x => x.GetFeedbackAsync(It.IsAny<Guid>())).ReturnsAsync((Feedback?)null);

        var handler = new GetFeedbackCommandHandler(repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new GetFeedbackCommand { Id = Guid.NewGuid() }, CancellationToken.None));
    }
}
