using Moq;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;

namespace StudyHub.UnitTests.Handlers.Feedbacks.Commands;

public class CreateFeedbackCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public CreateFeedbackCommandHandlerTests()
    {
        _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCreateFeedback_WhenRequestIsValid()
    {
        _feedbackRepositoryMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Name = "Ann", Surname = "Lee", UserName = "ann" };
        var expectedId = Guid.NewGuid();

        _feedbackRepositoryMock.Setup(x => x.AddFeedbackAsync(It.IsAny<Feedback>())).ReturnsAsync(expectedId);

        _userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);

        var handler = new CreateFeedbackCommandHandler(_feedbackRepositoryMock.Object, _userRepositoryMock.Object);

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
        _feedbackRepositoryMock.Verify(x => x.AddFeedbackAsync(It.Is<Feedback>(f =>
        f.User == user &&
        f.CreatorFullname == "Ann Lee" &&
        f.Description == "text")), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldCreateFeedback_WhenDescriptionContainsOnlyWhitespace()
    {
        _feedbackRepositoryMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        var user = new User { Id = Guid.NewGuid(), UserName = "usr" };
        _feedbackRepositoryMock.Setup(x => x.AddFeedbackAsync(It.IsAny<Feedback>())).ReturnsAsync(Guid.NewGuid());

        _userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);

        var handler = new CreateFeedbackCommandHandler(_feedbackRepositoryMock.Object, _userRepositoryMock.Object);

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
        _feedbackRepositoryMock.Verify(x => x.AddFeedbackAsync(It.Is<Feedback>(f => f.Description == string.Empty)), Times.Once);
    }
}



