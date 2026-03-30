using Moq;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Core.Subjects.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Queries;

public class GetAllQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnMappedSubjects()
    {
        // Arrange
        var items = new List<Subject> { new() { Id = Guid.NewGuid(), Name = "Math" } };
        var repositoryMock = new Mock<ISubjectRepository>();
        repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(items);

        var handler = new GetAllQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllSubjectsRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Math", result[0].Name);
    }
}
