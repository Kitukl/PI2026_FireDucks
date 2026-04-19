using Moq;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Commands;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;

namespace StudyHub.UnitTests.Handlers.Subjects.Commands;

public class AddSubjectCommandHandlerTests
{
    private readonly Mock<ISubjectRepository> _repositoryMock;

    public AddSubjectCommandHandlerTests()
    {
        _repositoryMock = new Mock<ISubjectRepository>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Test_1()
    {
        _repositoryMock.Reset();
        // Arrange
        var handler = new AddSubjectCommandHandler(_repositoryMock.Object);
        var dto = new SubjectDto { Id = Guid.NewGuid(), Name = "Math" };

        // Act
        await handler.Handle(new AddSubjectRequest(dto), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddSubject(It.Is<Subject>(s => s.Id == dto.Id && s.Name == "Math")), Times.Once);
    }
}

