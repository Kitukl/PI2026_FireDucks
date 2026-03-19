using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Lecturers.Queries
{
    public record GetByIdRequest(Guid id) : IRequest<LecturerDto?>;

    public class GetByIdHandler : IRequestHandler<GetByIdRequest, LecturerDto?>
    {
        private readonly ILecturerRepository _repo;

        public GetByIdHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task<LecturerDto?> Handle(GetByIdRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetById(request.id);
        }
    }
}