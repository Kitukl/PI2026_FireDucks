using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Schedules.Commands;

public class UpdateScheduleCommandHandlerTests
{
    private readonly Mock<IScheduleRepository> _repositoryMock;

    public UpdateScheduleCommandHandlerTests()
    {
        _repositoryMock = new Mock<IScheduleRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldUpdateSchedule_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        var scheduleId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new ScheduleDto
        {
            Id = scheduleId,
            IsAutoUpdate = false,
            LeaderUpdate = true,
            Lessons = new List<LessonDto> { new() { Id = lessonId } }
        };

        var handler = new UpdateScheduleCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new UpdateScheduleRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateScheduleAsync(It.Is<Schedule>(s =>
        s.Id == scheduleId &&
        s.Lessons.Count == 1 &&
        s.Lessons.First().Id == lessonId)), Times.Once);
    }
}



