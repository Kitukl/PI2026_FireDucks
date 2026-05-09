using Microsoft.Extensions.Configuration;
using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Group;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class AddScheduleCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;
    private readonly Mock<IGlobalAnnouncementService> _emailSenderMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public AddScheduleCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
        _emailSenderMock = new Mock<IGlobalAnnouncementService>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _configurationMock = new Mock<IConfiguration>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldAddSchedule_WhenRequestIsValid()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        
        _configurationMock
            .Setup(x => x["SendGrid:SchedulePage"])
            .Returns("https://studyhub.example.com/schedule");

        var handler = new AddScheduleCommandHandler(
            _repositoryMock.Object, 
            _emailSenderMock.Object, 
            _groupRepositoryMock.Object,
            _configurationMock.Object);

        var dto = new ScheduleDto
        {
            Id = Guid.Empty,
            Group = new GroupDto { Id = groupId, Name = "PI-21" },
            LeaderUpdate = true,
            IsAutoUpdate = true,
            UpdateAt = DateTime.UtcNow,
            Lessons = new List<LessonDto>
            {
                new()
                {
                    Id = Guid.Empty,
                    Day = DayOfWeek.Monday,
                    LessonType = "Lecture",
                    Subject = new SubjectDto { Id = Guid.Empty, Name = "Math" },
                    LessonSlot = new LessonSlotDto { Id = Guid.Empty, StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(9,0) },
                    Lecturers = new List<LecturerDtoResponse>()
                }
            }
        };

        var group = new Group 
        { 
            Id = groupId, 
            Name = "PI-21", 
            Users = new List<User> 
            { 
                new User { Email = "test@example.com" } 
            } 
        };

        _groupRepositoryMock
            .Setup(x => x.GetGroupByIdAsync(groupId))
            .ReturnsAsync(group);

        // Act
        await handler.Handle(new AddScheduleRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddSchedule(It.Is<Schedule>(s =>
            s.Group.Name == "PI-21" &&
            s.Lessons.Count == 1)), Times.Once);

        _emailSenderMock.Verify(x => x.SendGlobalAnnouncementAsync(
            It.Is<IReadOnlyCollection<string>>(emails => emails.Contains("test@example.com")),
            It.Is<string>(s => s.Contains("PI-21")),
            It.Is<string>(d => d.Contains("<div")),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}