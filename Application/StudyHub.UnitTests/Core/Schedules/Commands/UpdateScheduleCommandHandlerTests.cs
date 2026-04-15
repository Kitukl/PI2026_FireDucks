using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class UpdateScheduleCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapAndUpdateSchedule()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new ScheduleDto
        {
            Id = scheduleId,
            IsAutoUpdate = false,
            HeadmanUpdate = true,
            Lessons = new List<LessonDto> { new() { Id = lessonId } }
        };

        var repositoryMock = new Mock<IScheduleRepository>();
        var handler = new UpdateScheduleCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateScheduleRequest(dto), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.UpdateScheduleAsync(It.Is<Schedule>(s =>
            s.Id == scheduleId &&
            s.Lessons.Count == 1 &&
            s.Lessons.First().Id == lessonId)), Times.Once);
    }
}
