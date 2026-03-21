using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.LessonSlots.Commands
{
    public record AddLessonSlotRequest(LessonSlotDto lessonSlot) : IRequest;

    public class AddLessonSlotCommandHandler : IRequestHandler<AddLessonSlotRequest>
    {
        private readonly ILessonSlotRepository _repo;

        public AddLessonSlotCommandHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddLessonSlotRequest request, CancellationToken cancellationToken)
        {
            var lessonSlot = new LessonsSlot
            {
                Id = request.lessonSlot.Id == Guid.Empty ? Guid.NewGuid() : request.lessonSlot.Id,
                StartTime = request.lessonSlot.StartTime,
                EndTime = request.lessonSlot.EndTime
            };

            await _repo.AddLessonSlot(lessonSlot);
        }
    }
}