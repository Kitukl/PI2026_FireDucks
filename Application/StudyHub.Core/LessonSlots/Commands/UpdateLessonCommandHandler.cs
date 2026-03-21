using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.LessonSlots.Commands
{
    public record UpdateLessonSlotRequest(LessonSlotDto lessonSlot) : IRequest;

    public class UpdateLessonSlotCommandHandler : IRequestHandler<UpdateLessonSlotRequest>
    {
        private readonly ILessonSlotRepository _repo;

        public UpdateLessonSlotCommandHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateLessonSlotRequest request, CancellationToken cancellationToken)
        {
            var lessonSlot = new LessonsSlot
            {
                Id = request.lessonSlot.Id == Guid.Empty ? Guid.NewGuid() : request.lessonSlot.Id,
                StartTime = request.lessonSlot.StartTime,
                EndTime = request.lessonSlot.EndTime
            };

            await _repo.UpdateLessonSlot(lessonSlot);
        }
    }
}