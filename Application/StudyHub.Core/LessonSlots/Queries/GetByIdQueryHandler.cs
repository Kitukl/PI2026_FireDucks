using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;

namespace StudyHub.Core.LessonSlots.Queries
{
    public record GetLessonSlotByIdRequest(Guid id) : IRequest<LessonSlotDto?>;

    public class GetByIdQueryHandler : IRequestHandler<GetLessonSlotByIdRequest, LessonSlotDto?>
    {
        private readonly ILessonSlotRepository _repo;

        public GetByIdQueryHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task<LessonSlotDto?> Handle(GetLessonSlotByIdRequest request, CancellationToken cancellationToken)
        {
            var lessonSlot = await _repo.GetById(request.id);

            if (lessonSlot != null)
            {
                var lessonSlotDto = new LessonSlotDto
                {
                    Id = lessonSlot.Id == Guid.Empty ? Guid.NewGuid() : lessonSlot.Id,
                    StartTime = lessonSlot.StartTime,
                    EndTime = lessonSlot.EndTime
                };

                return lessonSlotDto;
            }

            return null;
        }
    }
}