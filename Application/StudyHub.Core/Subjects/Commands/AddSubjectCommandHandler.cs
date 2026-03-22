using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Subjects.Commands
{
    public record AddSubjectRequest(SubjectDto subject) : IRequest;

    public class AddSubjectCommandHandler : IRequestHandler<AddSubjectRequest>
    {
        private readonly ISubjectRepository _repo;

        public AddSubjectCommandHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddSubjectRequest request, CancellationToken cancellationToken)
        {
            var subject = new Subject
            {
                Id = request.subject.Id,
                Name = request.subject.Name
            };

            await _repo.AddSubject(subject);
        }
    }
}