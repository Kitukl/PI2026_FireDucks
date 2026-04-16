using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Commands;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Commands;

public class AddSubjectCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldMapAndCallAdd()
    {
        // Arrange
        var repositoryMock = new Mock<ISubjectRepository>();
        var handler = new AddSubjectCommandHandler(repositoryMock.Object);
        var dto = new SubjectDto { Id = Guid.NewGuid(), Name = "Math" };

        // Act
        await handler.Handle(new AddSubjectRequest(dto), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.AddSubject(It.Is<Subject>(s => s.Id == dto.Id && s.Name == "Math")), Times.Once);
    }
}
