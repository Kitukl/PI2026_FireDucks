using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Interfaces;

namespace StudyHub.Core.Lecturers.Queries
{
    public record GetAllRequest : IRequest<List<LecturerDtoResponse>>;

    public class GetAllQueryHandler : IRequestHandler<GetAllRequest, List<LecturerDtoResponse>>
    {
        private readonly ILecturerRepository _repo;

        public GetAllQueryHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LecturerDtoResponse>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return (await _repo.GetAll())
                .Select(x => new LecturerDtoResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname
                }).ToList();
        }
    }
}