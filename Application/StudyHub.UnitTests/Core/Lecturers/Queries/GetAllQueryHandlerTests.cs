using Moq;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Core.Lecturers.Queries;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Lecturers.Queries;

public class GetAllQueryHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnMappedLecturers()
    {
        // Arrange
        var repositoryMock = new Mock<ILecturerRepository>();
        repositoryMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Lecturer>
        {
            new() { Id = Guid.NewGuid(), Name = "A", Surname = "B" }
        });

        var handler = new GetAllQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetAllLecturersRequest(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("A", result[0].Name);
    }
}
