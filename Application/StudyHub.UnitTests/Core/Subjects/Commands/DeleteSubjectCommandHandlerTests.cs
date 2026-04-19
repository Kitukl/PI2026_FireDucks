using Moq;
using StudyHub.Core.Subjects.Commands;
using StudyHub.Core.Subjects.Interfaces;

namespace StudyHub.UnitTests.Handlers.Subjects.Commands;

public class DeleteSubjectCommandHandlerTests
{
    private readonly Mock<ISubjectRepository> _repositoryMock;

    public DeleteSubjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<ISubjectRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var id = Guid.NewGuid();
        var handler = new DeleteSubjectCommandHandler(_repositoryMock.Object);

        // Act
        await handler.Handle(new DeleteSubjectRequest(id), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.DeleteSubject(id), Times.Once);
    }
}

