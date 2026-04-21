using Moq;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Core.Lecturers.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lecturers.Queries;

public class GetAllQueryHandlerTests
{
    private readonly Mock<ILecturerRepository> _repositoryMock;

    public GetAllQueryHandlerTests()
    {
        _repositoryMock = new Mock<ILecturerRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnLecturers_WhenRequestIsValid()
    {
        _repositoryMock.Reset();
        // Arrange
        _repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Lecturer>
        {
        new() { Id = Guid.NewGuid(), Name = "A", Surname = "B" }
        });

        var handler = new GetAllQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllLecturersRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("A", result[0].Name);
    }
}



