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

namespace StudyHub.Core.Lecturers.Commands
{
    public record UpdateLecturerRequest(LecturerDto lecturer) : IRequest;

    public class UpdateLecturerHandler : IRequestHandler<UpdateLecturerRequest>
    {
        private readonly ILecturerRepository _repo;

        public UpdateLecturerHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateLecturerRequest request, CancellationToken cancellationToken)
        {
            await _repo.UpdateLecturer(request.lecturer);
        }
    }
}