using Moq;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.LessonSlots.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.LessonSlots.Queries;

public class GetByIdQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFound_ShouldReturnDto()
    {
        // Arrange
        var slot = new LessonsSlot { Id = Guid.Empty, StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(8, 0) };
        var repositoryMock = new Mock<ILessonSlotRepository>();
        repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(slot);

        var handler = new GetByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLessonSlotByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result!.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenMissing_ShouldReturnNull()
    {
        // Arrange
        var repositoryMock = new Mock<ILessonSlotRepository>();
        repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((LessonsSlot?)null);

        var handler = new GetByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLessonSlotByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
