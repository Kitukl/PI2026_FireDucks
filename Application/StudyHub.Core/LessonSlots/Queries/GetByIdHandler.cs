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
    public record GetByIdRequest(Guid id) : IRequest<LessonSlotDto?>;

    public class GetByIdHandler : IRequestHandler<GetByIdRequest, LessonSlotDto?>
    {
        private readonly ILessonSlotRepository _repo;

        public GetByIdHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task<LessonSlotDto?> Handle(GetByIdRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetById(request.id);
        }
    }
}