using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Commands;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Commands;

public class UpdateSubjectCommandHandlerTests
{
    private readonly Mock<ISubjectRepository> _repositoryMock;

    public UpdateSubjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<ISubjectRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new UpdateSubjectCommandHandler(_repositoryMock.Object);
        var dto = new SubjectDto { Id = Guid.NewGuid(), Name = "Physics" };

        // Act
        await handler.Handle(new UpdateSubjectRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.UpdateSubject(It.Is<Subject>(s => s.Id == dto.Id && s.Name == "Physics")), Times.Once);
    }
}

