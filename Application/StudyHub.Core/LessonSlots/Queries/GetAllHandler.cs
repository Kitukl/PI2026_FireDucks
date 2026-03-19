using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.LessonSlots.Queries
{
    public record GetAllRequest : IRequest<List<LessonSlotDto?>>;

    public class GetAllHandler : IRequestHandler<GetAllRequest, List<LessonSlotDto?>>
    {
        private readonly ILessonSlotRepository _repo;

        public GetAllHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LessonSlotDto?>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetAll();
        }
    }
}