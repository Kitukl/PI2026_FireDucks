using Moq;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.LessonSlots.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.LessonSlots.Queries;

public class GetAllQueryHandlerTests
{
    private readonly Mock<ILessonSlotRepository> _repositoryMock;

    public GetAllQueryHandlerTests()
    {
        _repositoryMock = new Mock<ILessonSlotRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var slots = new List<LessonsSlot>
        {
        new() { Id = Guid.Empty, StartTime = new TimeOnly(8,0), EndTime = new TimeOnly(9,0) }
        };

        _repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(slots);

        var handler = new GetAllQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllLessonSlotsRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.NotEqual(Guid.Empty, result[0].Id);
    }
}

