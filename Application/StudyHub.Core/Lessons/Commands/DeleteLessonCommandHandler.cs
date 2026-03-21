using MediatR;
using StudyHub.Core.Lessons.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Lessons.Commands
{
    public record DeleteLessonRequest(Guid id) : IRequest;

    public class DeleteLessonCommandHandler : IRequestHandler<DeleteLessonRequest>
    {
        private readonly ILessonRepository _repo;

        public DeleteLessonCommandHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteLessonRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteLesson(request.id);
        }
    }
}