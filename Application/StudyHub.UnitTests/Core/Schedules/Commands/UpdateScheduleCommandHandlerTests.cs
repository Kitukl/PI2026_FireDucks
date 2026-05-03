using Microsoft.Extensions.Configuration;
using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Group;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class UpdateScheduleCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;
    private readonly Mock<IGlobalAnnouncementService> _emailSenderMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public UpdateScheduleCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
        _emailSenderMock = new Mock<IGlobalAnnouncementService>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _configurationMock = new Mock<IConfiguration>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateSchedule_WhenRequestIsValid()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        _configurationMock
            .Setup(x => x["SendGrid:SchedulePage"])
            .Returns("https://studyhub.example.com/schedule");

        var dto = new ScheduleDto
        {
            Id = scheduleId,
            Group = new GroupDto { Id = groupId, Name = "ПМІ-31" },
            IsAutoUpdate = false,
            LeaderUpdate = true,
            Lessons = new List<LessonDto> { new() { Id = lessonId } }
        };

        var group = new Group 
        { 
            Id = groupId, 
            Name = "ПМІ-31", 
            Users = new List<User> { new User { Email = "student@test.com" } } 
        };

        _groupRepositoryMock
            .Setup(x => x.GetGroupByIdAsync(groupId))
            .ReturnsAsync(group);

        var handler = new UpdateScheduleCommandHandler(
            _repositoryMock.Object, 
            _emailSenderMock.Object, 
            _groupRepositoryMock.Object, 
            _configurationMock.Object);

        // Act
        await handler.Handle(new UpdateScheduleRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateScheduleAsync(It.Is<Schedule>(s =>
            s.Id == scheduleId &&
            s.Lessons.Count == 1 &&
            s.Lessons.First().Id == lessonId)), Times.Once);

        _emailSenderMock.Verify(x => x.SendGlobalAnnouncementAsync(
            It.Is<IReadOnlyCollection<string>>(emails => emails.Contains("student@test.com")),
            It.IsAny<string>(),
            It.Is<string>(html => html.Contains("ПМІ-31")),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}