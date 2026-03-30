using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Commands;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Commands;

public class UpdateSubjectCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapAndCallUpdate()
    {
        // Arrange
        var repositoryMock = new Mock<ISubjectRepository>();
        var handler = new UpdateSubjectCommandHandler(repositoryMock.Object);
        var dto = new SubjectDto { Id = Guid.NewGuid(), Name = "Physics" };

        // Act
        await handler.Handle(new UpdateSubjectRequest(dto), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.UpdateSubject(It.Is<Subject>(s => s.Id == dto.Id && s.Name == "Physics")), Times.Once);
    }
}
