using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Interfaces;

namespace StudyHub.Core.Subjects.Queries
{
    public record GetSubjectByIdRequest(Guid id) : IRequest<SubjectDto?>;

    public class GetByIdQueryHandler : IRequestHandler<GetSubjectByIdRequest, SubjectDto?>
    {
        private readonly ISubjectRepository _repo;

        public GetByIdQueryHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task<SubjectDto?> Handle(GetSubjectByIdRequest request, CancellationToken cancellationToken)
        {
            var subject = await _repo.GetById(request.id);

            if (subject != null)
            {
                var subjectDto = new SubjectDto
                {
                    Id = subject.Id,
                    Name = subject.Name
                };

                return subjectDto;
            }

            return null;
        }
    }
}