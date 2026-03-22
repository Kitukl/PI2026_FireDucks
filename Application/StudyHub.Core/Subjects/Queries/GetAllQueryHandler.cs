using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Interfaces;

namespace StudyHub.Core.Subjects.Queries
{
    public record GetAllSubjectsRequest : IRequest<List<SubjectDto>>;

    public class GetAllQueryHandler : IRequestHandler<GetAllSubjectsRequest, List<SubjectDto>>
    {
        private readonly ISubjectRepository _repo;

        public GetAllQueryHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SubjectDto>> Handle(GetAllSubjectsRequest request, CancellationToken cancellationToken)
        {
            return (await _repo.GetAll())
                .Select(x => new SubjectDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
        }
    }
}