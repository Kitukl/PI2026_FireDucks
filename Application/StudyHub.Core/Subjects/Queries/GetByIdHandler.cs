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
    public record GetByIdRequest(Guid id) : IRequest<SubjectDto?>;

    public class GetByIdHandler : IRequestHandler<GetByIdRequest, SubjectDto?>
    {
        private readonly ISubjectRepository _repo;

        public GetByIdHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<SubjectDto?> Handle(GetByIdRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetById(request.id);
        }
    }
}