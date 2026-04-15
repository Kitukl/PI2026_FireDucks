using Moq;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Queries;

public class GetByIdQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenFound_ShouldReturnDto()
    {
        // Arrange
        var item = new Subject { Id = Guid.NewGuid(), Name = "Math" };
        var repositoryMock = new Mock<ISubjectRepository>();
        repositoryMock.Setup(x => x.GetById(item.Id)).ReturnsAsync(item);

        var handler = new GetByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetSubjectByIdRequest(item.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Name, result!.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var repositoryMock = new Mock<ISubjectRepository>();
        repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((Subject?)null);

        var handler = new GetByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetSubjectByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
