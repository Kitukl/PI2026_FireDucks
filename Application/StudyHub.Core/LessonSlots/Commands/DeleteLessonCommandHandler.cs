using MediatR;
using StudyHub.Core.LessonSlots.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.LessonSlots.Commands
{
    public record DeleteLessonSlotRequest(Guid id) : IRequest;

    public class DeleteLessonSlotCommandHandler : IRequestHandler<DeleteLessonSlotRequest>
    {
        private readonly ILessonSlotRepository _repo;

        public DeleteLessonSlotCommandHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteLessonSlotRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteLessonSlot(request.id);
        }
    }
}