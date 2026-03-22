using MediatR;
using StudyHub.Core.Lecturers.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Lecturers.Commands
{
    public record DeleteLecturerRequest(Guid id) : IRequest;

    public class DeleteLecturerCommandHandler : IRequestHandler<DeleteLecturerRequest>
    {
        private readonly ILecturerRepository _repo;

        public DeleteLecturerCommandHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteLecturerRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteLecturer(request.id);
        }
    }
}