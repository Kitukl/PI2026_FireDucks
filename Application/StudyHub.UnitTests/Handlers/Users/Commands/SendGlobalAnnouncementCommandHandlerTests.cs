using Moq;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class SendGlobalAnnouncementCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenSubjectOrDescriptionIsEmpty_ShouldReturnZero()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        var senderMock = new Mock<IGlobalAnnouncementService>();
        var handler = new SendGlobalAnnouncementCommandHandler(userRepositoryMock.Object, senderMock.Object);

        // Act
        var result = await handler.Handle(new SendGlobalAnnouncementCommand { Subject = "   ", Description = "text" }, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
        userRepositoryMock.Verify(x => x.GetAllEmailsAsync(), Times.Never);
        senderMock.Verify(x => x.SendGlobalAnnouncementAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenNoRecipients_ShouldReturnZero()
    {
        // Arrange
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetAllEmailsAsync()).ReturnsAsync(new List<string>());
        var senderMock = new Mock<IGlobalAnnouncementService>();
        var handler = new SendGlobalAnnouncementCommandHandler(userRepositoryMock.Object, senderMock.Object);

        // Act
        var result = await handler.Handle(new SendGlobalAnnouncementCommand { Subject = "Subject", Description = "Body" }, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
        senderMock.Verify(x => x.SendGlobalAnnouncementAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenValid_ShouldSendAndReturnRecipientsCount()
    {
        // Arrange
        var recipients = new List<string> { "a@a.com", "b@b.com" };
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(x => x.GetAllEmailsAsync()).ReturnsAsync(recipients);

        var senderMock = new Mock<IGlobalAnnouncementService>();
        var handler = new SendGlobalAnnouncementCommandHandler(userRepositoryMock.Object, senderMock.Object);

        // Act
        var result = await handler.Handle(new SendGlobalAnnouncementCommand { Subject = " Subject ", Description = " Body " }, CancellationToken.None);

        // Assert
        Assert.Equal(2, result);
        senderMock.Verify(x => x.SendGlobalAnnouncementAsync(
            It.Is<IReadOnlyCollection<string>>(x => x.Count == 2),
            "Subject",
            "Body",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

