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
    public record GetAllRequest : IRequest<List<LecturerDto?>>;

    public class GetAllHandler : IRequestHandler<GetAllRequest, List<LecturerDto?>>
    {
        private readonly ILecturerRepository _repo;

        public GetAllHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LecturerDto?>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetAll();
        }
    }
}