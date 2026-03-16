using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Lessons.Queries
{
    public record GetByIdRequest(Guid id) : IRequest<LessonDto?>;

    public class GetByIdHandler : IRequestHandler<GetByIdRequest, LessonDto?>
    {
        private readonly ILessonRepository _repo;

        public GetByIdHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task<LessonDto?> Handle(GetByIdRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetById(request.id);
        }
    }
}