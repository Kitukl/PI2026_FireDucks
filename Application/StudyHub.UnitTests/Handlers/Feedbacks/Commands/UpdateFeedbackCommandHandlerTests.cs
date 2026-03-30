using Moq;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Feedbacks.Commands;

public class UpdateFeedbackCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenResolved_ShouldSetResolvedAt()
    {
        // Arrange
        var feedback = new Feedback { Id = Guid.NewGuid(), Status = Status.ToDo, Description = "x", CreatorFullname = "u", User = new User() };

        var repositoryMock = new Mock<IFeedbackRepository>();
        repositoryMock.Setup(x => x.GetFeedbackAsync(feedback.Id)).ReturnsAsync(feedback);
        repositoryMock.Setup(x => x.UpdateFeedbackAsync(feedback)).ReturnsAsync(feedback.Id);

        var handler = new UpdateFeedbackCommandHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new UpdateFeedbackCommand { Id = feedback.Id, Status = Status.Resolved }, CancellationToken.None);

        // Assert
        Assert.Equal(feedback.Id, result);
        Assert.Equal(Status.Resolved, feedback.Status);
        Assert.NotNull(feedback.UpdatedAt);
        Assert.NotNull(feedback.ResolvedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenNotResolved_ShouldClearResolvedAt()
    {
        // Arrange
        var feedback = new Feedback { Id = Guid.NewGuid(), Status = Status.Resolved, ResolvedAt = DateTime.UtcNow, Description = "x", CreatorFullname = "u", User = new User() };

        var repositoryMock = new Mock<IFeedbackRepository>();
        repositoryMock.Setup(x => x.GetFeedbackAsync(feedback.Id)).ReturnsAsync(feedback);
        repositoryMock.Setup(x => x.UpdateFeedbackAsync(feedback)).ReturnsAsync(feedback.Id);

        var handler = new UpdateFeedbackCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateFeedbackCommand { Id = feedback.Id, Status = Status.InProgress }, CancellationToken.None);

        // Assert
        Assert.Null(feedback.ResolvedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenMissing_ShouldThrow()
    {
        // Arrange
        var repositoryMock = new Mock<IFeedbackRepository>();
        repositoryMock.Setup(x => x.GetFeedbackAsync(It.IsAny<Guid>())).ReturnsAsync((Feedback?)null);

        var handler = new UpdateFeedbackCommandHandler(repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new UpdateFeedbackCommand { Id = Guid.NewGuid(), Status = Status.ToDo }, CancellationToken.None));
    }
}
