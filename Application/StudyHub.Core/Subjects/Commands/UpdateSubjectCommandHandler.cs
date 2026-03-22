using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Subjects.Commands
{
    public record UpdateSubjectRequest(SubjectDto subject) : IRequest;

    public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectRequest>
    {
        private readonly ISubjectRepository _repo;

        public UpdateSubjectCommandHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateSubjectRequest request, CancellationToken cancellationToken)
        {
            var subject = new Subject
            {
                Id = request.subject.Id,
                Name = request.subject.Name
            };

            await _repo.UpdateSubject(subject);
        }
    }
}