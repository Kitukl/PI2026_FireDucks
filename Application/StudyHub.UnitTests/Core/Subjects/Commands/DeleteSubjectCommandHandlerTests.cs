using Moq;
using StudyHub.Core.Subjects.Commands;
using StudyHub.Core.Subjects.Interfaces;

namespace StudyHub.UnitTests.Handlers.Subjects.Commands;

public class DeleteSubjectCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteById()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<ISubjectRepository>();
        var handler = new DeleteSubjectCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteSubjectRequest(id), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.DeleteSubject(id), Times.Once);
    }
}
