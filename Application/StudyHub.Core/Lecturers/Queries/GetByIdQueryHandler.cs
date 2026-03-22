using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Interfaces;

namespace StudyHub.Core.Lecturers.Queries
{
    public record GetLecturerByIdRequest(Guid id) : IRequest<LecturerDtoResponse?>;

    public class GetByIdQueryHandler : IRequestHandler<GetLecturerByIdRequest, LecturerDtoResponse?>
    {
        private readonly ILecturerRepository _repo;

        public GetByIdQueryHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task<LecturerDtoResponse?> Handle(GetLecturerByIdRequest request, CancellationToken cancellationToken)
        {
            var lecturer = await _repo.GetById(request.id);

            if (lecturer != null)
            {
                var lecturerDto = new LecturerDtoResponse
                {
                    Id = lecturer.Id,
                    Name = lecturer.Name,
                    Surname = lecturer.Surname
                };

                return lecturerDto;
            }
            return null;
        }
    }
}