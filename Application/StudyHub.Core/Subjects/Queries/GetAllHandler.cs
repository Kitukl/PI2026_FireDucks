using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Schedule.Interfaces;
using StudyHub.Core.Subjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Subjects.Queries
{
    public record GetAllRequest : IRequest<List<SubjectDto?>>;

    public class GetAllHandler : IRequestHandler<GetAllRequest, List<SubjectDto?>>
    {
        private readonly ISubjectRepository _repo;

        public GetAllHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SubjectDto?>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetAll();
        }
    }
}