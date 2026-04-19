using Moq;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Feedbacks.Commands;

public class UpdateFeedbackCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _repositoryMock;

    public UpdateFeedbackCommandHandlerTests()
    {
        _repositoryMock = new Mock<IFeedbackRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var feedback = new Feedback { Id = Guid.NewGuid(), Status = Status.ToDo, Description = "x", CreatorFullname = "u", User = new User() };

        _repositoryMock.Setup(x => x.GetFeedbackAsync(feedback.Id)).ReturnsAsync(feedback);
        _repositoryMock.Setup(x => x.UpdateFeedbackAsync(feedback)).ReturnsAsync(feedback.Id);

        var handler = new UpdateFeedbackCommandHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new UpdateFeedbackCommand { Id = feedback.Id, Status = Status.Resolved }, CancellationToken.None);

        // Assert
        Assert.Equal(feedback.Id, result);
        Assert.Equal(Status.Resolved, feedback.Status);
        Assert.NotNull(feedback.UpdatedAt);
        Assert.NotNull(feedback.ResolvedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_2()
    {
        _repositoryMock.Reset();
        // Arrange
        var feedback = new Feedback { Id = Guid.NewGuid(), Status = Status.Resolved, ResolvedAt = DateTime.UtcNow, Description = "x", CreatorFullname = "u", User = new User() };

        _repositoryMock.Setup(x => x.GetFeedbackAsync(feedback.Id)).ReturnsAsync(feedback);
        _repositoryMock.Setup(x => x.UpdateFeedbackAsync(feedback)).ReturnsAsync(feedback.Id);

        var handler = new UpdateFeedbackCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateFeedbackCommand { Id = feedback.Id, Status = Status.InProgress }, CancellationToken.None);

        // Assert
        Assert.Null(feedback.ResolvedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_3()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetFeedbackAsync(It.IsAny<Guid>())).ReturnsAsync((Feedback?)null);

        var handler = new UpdateFeedbackCommandHandler(_repositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(new UpdateFeedbackCommand { Id = Guid.NewGuid(), Status = Status.ToDo }, CancellationToken.None));
    }
}

