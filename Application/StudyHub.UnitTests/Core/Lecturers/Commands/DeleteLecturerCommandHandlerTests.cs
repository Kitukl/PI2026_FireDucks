using Moq;
using StudyHub.Core.Lecturers.Commands;
using StudyHub.Core.Lecturers.Interfaces;

namespace StudyHub.UnitTests.Handlers.Lecturers.Commands;

public class DeleteLecturerCommandHandlerTests
{
    private readonly Mock<ILecturerRepository> _repositoryMock;

    public DeleteLecturerCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILecturerRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        var handler = new DeleteLecturerCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteLecturerRequest(id), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.DeleteLecturer(id), Times.Once);
    }
}

