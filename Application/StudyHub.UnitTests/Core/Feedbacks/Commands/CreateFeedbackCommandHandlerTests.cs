using Moq;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Feedbacks.Commands;

public class CreateFeedbackCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCreateFeedbackWithTrimmedDescription()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Ann", Surname = "Lee", UserName = "ann" };
        var expectedId = Guid.NewGuid();

        var feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        feedbackRepositoryMock.Setup(x => x.AddFeedbackAsync(It.IsAny<Feedback>())).ReturnsAsync(expectedId);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);

        var handler = new CreateFeedbackCommandHandler(feedbackRepositoryMock.Object, userRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new CreateFeedbackCommand
        {
            UserId = user.Id,
            Category = Category.General,
            FeedbackType = FeedbackType.Issue,
            Status = Status.ToDo,
            Description = "  text  "
        }, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        feedbackRepositoryMock.Verify(x => x.AddFeedbackAsync(It.Is<Feedback>(f =>
            f.User == user &&
            f.CreatorFullname == "Ann Lee" &&
            f.Description == "text")), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenDescriptionWhitespace_ShouldStoreEmptyString()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), UserName = "usr" };
        var feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        feedbackRepositoryMock.Setup(x => x.AddFeedbackAsync(It.IsAny<Feedback>())).ReturnsAsync(Guid.NewGuid());

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);

        var handler = new CreateFeedbackCommandHandler(feedbackRepositoryMock.Object, userRepositoryMock.Object);

        // Act
        await handler.Handle(new CreateFeedbackCommand
        {
            UserId = user.Id,
            Category = Category.General,
            FeedbackType = FeedbackType.Request,
            Status = Status.ToDo,
            Description = "   "
        }, CancellationToken.None);

        // Assert
        feedbackRepositoryMock.Verify(x => x.AddFeedbackAsync(It.Is<Feedback>(f => f.Description == string.Empty)), Times.Once);
    }
}

