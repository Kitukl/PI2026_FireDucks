using Moq;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Queries;

public class GetAllQueryHandlerTests
{
    private readonly Mock<ISubjectRepository> _repositoryMock;

    public GetAllQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISubjectRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var items = new List<Subject> { new() { Id = Guid.NewGuid(), Name = "Math" } };
        _repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(items);

        var handler = new GetAllQueryHandler(_repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllSubjectsRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Math", result[0].Name);
    }
}

