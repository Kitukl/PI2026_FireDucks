using Moq;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Core.Users.Commands;
using StudyHub.Core.Users.Interfaces;

namespace StudyHub.UnitTests.Handlers.Users.Commands;

public class SendGlobalAnnouncementCommandHandlerTests
{
    private readonly Mock<IGlobalAnnouncementService> _senderMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public SendGlobalAnnouncementCommandHandlerTests()
    {
        _senderMock = new Mock<IGlobalAnnouncementService>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _senderMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        var handler = new SendGlobalAnnouncementCommandHandler(_userRepositoryMock.Object, _senderMock.Object);

        // Act
        var result = await handler.Handle(new SendGlobalAnnouncementCommand { Subject = "   ", Description = "text" }, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
        _userRepositoryMock.Verify(x => x.GetAllEmailsAsync(), Times.Never);
        _senderMock.Verify(x => x.SendGlobalAnnouncementAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_2()
    {
        _senderMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        _userRepositoryMock.Setup(x => x.GetAllEmailsAsync()).ReturnsAsync(new List<string>());
        var handler = new SendGlobalAnnouncementCommandHandler(_userRepositoryMock.Object, _senderMock.Object);

        // Act
        var result = await handler.Handle(new SendGlobalAnnouncementCommand { Subject = "Subject", Description = "Body" }, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
        _senderMock.Verify(x => x.SendGlobalAnnouncementAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_3()
    {
        _senderMock.Reset();
        _userRepositoryMock.Reset();
        // Arrange
        var recipients = new List<string> { "a@a.com", "b@b.com" };
        _userRepositoryMock.Setup(x => x.GetAllEmailsAsync()).ReturnsAsync(recipients);

        var handler = new SendGlobalAnnouncementCommandHandler(_userRepositoryMock.Object, _senderMock.Object);

        // Act
        var result = await handler.Handle(new SendGlobalAnnouncementCommand { Subject = " Subject ", Description = " Body " }, CancellationToken.None);

        // Assert
        Assert.Equal(2, result);
        _senderMock.Verify(x => x.SendGlobalAnnouncementAsync(
        It.Is<IReadOnlyCollection<string>>(x => x.Count == 2),
        "Subject",
        "Body",
        It.IsAny<CancellationToken>()), Times.Once);
    }
}

