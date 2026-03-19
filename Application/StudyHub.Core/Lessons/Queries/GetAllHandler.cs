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
    public record GetAllRequest : IRequest<List<LessonDto?>>;

    public class GetAllHandler : IRequestHandler<GetAllRequest, List<LessonDto?>>
    {
        private readonly ILessonRepository _repo;

        public GetAllHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LessonDto?>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetAll();
        }
    }
}