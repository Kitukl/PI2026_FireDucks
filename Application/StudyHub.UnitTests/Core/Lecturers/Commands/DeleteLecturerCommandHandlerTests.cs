using Moq;
using StudyHub.Core.Lecturers.Commands;
using StudyHub.Core.Lecturers.Interfaces;

namespace StudyHub.UnitTests.Handlers.Lecturers.Commands;

public class DeleteLecturerCommandHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldDeleteLecturer()
    {
        // Arrange
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<ILecturerRepository>();
        var handler = new DeleteLecturerCommandHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteLecturerRequest(id), CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.DeleteLecturer(id), Times.Once);
    }
}
