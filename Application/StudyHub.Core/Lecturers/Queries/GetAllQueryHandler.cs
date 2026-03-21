using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Interfaces;

namespace StudyHub.Core.Lecturers.Queries
{
    public record GetAllLecturersRequest : IRequest<List<LecturerDtoResponse>>;

    public class GetAllQueryHandler : IRequestHandler<GetAllLecturersRequest, List<LecturerDtoResponse>>
    {
        private readonly ILecturerRepository _repo;

        public GetAllQueryHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LecturerDtoResponse>> Handle(GetAllLecturersRequest request, CancellationToken cancellationToken)
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