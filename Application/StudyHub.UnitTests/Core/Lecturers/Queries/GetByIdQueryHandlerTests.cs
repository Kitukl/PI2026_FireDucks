using Moq;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Core.Lecturers.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lecturers.Queries;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<ILecturerRepository> _repositoryMock;

    public GetByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ILecturerRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var entity = new Lecturer { Id = Guid.NewGuid(), Name = "A", Surname = "B" };
        _repositoryMock.Setup(x => x.GetById(entity.Id)).ReturnsAsync(entity);

        var handler = new GetByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLecturerByIdRequest(entity.Id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result!.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_2()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((Lecturer?)null);

        var handler = new GetByIdQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLecturerByIdRequest(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}

