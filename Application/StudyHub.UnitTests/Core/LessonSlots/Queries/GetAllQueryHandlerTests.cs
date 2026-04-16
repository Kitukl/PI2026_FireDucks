using Moq;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.LessonSlots.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.LessonSlots.Queries;

public class GetAllQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnMappedSlots()
    {
        // Arrange
        var slots = new List<LessonsSlot>
        {
            new() { Id = Guid.Empty, StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(9,0) }
        };

        var repositoryMock = new Mock<ILessonSlotRepository>();
        repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(slots);

        var handler = new GetAllQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllLessonSlotsRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.NotEqual(Guid.Empty, result[0].Id);
    }
}
