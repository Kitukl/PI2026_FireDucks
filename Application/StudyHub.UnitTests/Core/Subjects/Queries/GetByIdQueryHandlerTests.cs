using Moq;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Queries;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<ISubjectRepository> _repositoryMock;

    public GetByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISubjectRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnSubject_WhenSubjectExists()
    {
        _repositoryMock.Reset();
        // Arrange
        var item = new Subject { Id = Guid.NewGuid(), Name = "Math" };
        _repositoryMock.Setup(x => x.GetById(item.Id)).ReturnsAsync(item);

        var handler = new GetByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetSubjectByIdRequest(item.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Name, result!.Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldGetById_WhenSubjectNotFound()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((Subject?)null);

        var handler = new GetByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetSubjectByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}



