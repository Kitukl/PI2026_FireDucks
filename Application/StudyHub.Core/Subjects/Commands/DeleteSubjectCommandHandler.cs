using MediatR;
using StudyHub.Core.Subjects.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Subjects.Commands
{
    public record DeleteSubjectRequest(Guid id) : IRequest;

    public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectRequest>
    {
        private readonly ISubjectRepository _repo;

        public DeleteSubjectCommandHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteSubjectRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteSubject(request.id);
        }
    }
}