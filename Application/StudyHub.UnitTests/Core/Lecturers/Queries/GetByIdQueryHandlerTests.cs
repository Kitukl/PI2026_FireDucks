using Moq;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Core.Lecturers.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lecturers.Queries;

public class GetByIdQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFound_ShouldReturnDto()
    {
        // Arrange
        var entity = new Lecturer { Id = Guid.NewGuid(), Name = "A", Surname = "B" };
        var repositoryMock = new Mock<ILecturerRepository>();
        repositoryMock.Setup(x => x.GetById(entity.Id)).ReturnsAsync(entity);

        var handler = new GetByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLecturerByIdRequest(entity.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result!.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var repositoryMock = new Mock<ILecturerRepository>();
        repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((Lecturer?)null);

        var handler = new GetByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLecturerByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
