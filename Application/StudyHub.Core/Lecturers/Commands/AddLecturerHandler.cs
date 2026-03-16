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
    public record AddLecturerRequest(LecturerDto lecturer) : IRequest;

    public class AddLecturerHandler : IRequestHandler<AddLecturerRequest>
    {
        private readonly ILecturerRepository _repo;

        public AddLecturerHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddLecturerRequest request, CancellationToken cancellationToken)
        {
            await _repo.AddLecturer(request.lecturer);
        }
    }
}