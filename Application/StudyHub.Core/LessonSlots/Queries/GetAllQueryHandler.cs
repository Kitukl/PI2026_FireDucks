using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Interfaces;

namespace StudyHub.Core.LessonSlots.Queries
{
    public record GetAllLessonSlotsRequest : IRequest<List<LessonSlotDto>>;

    public class GetAllQueryHandler : IRequestHandler<GetAllLessonSlotsRequest, List<LessonSlotDto>>
    {
        private readonly ILessonSlotRepository _repo;

        public GetAllQueryHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LessonSlotDto>> Handle(GetAllLessonSlotsRequest request, CancellationToken cancellationToken)
        {
            return (await _repo.GetAll())
                .Select(x => new LessonSlotDto
                {
                    Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                }).ToList();
        }
    }
}