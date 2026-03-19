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
    public record DeleteLecturerRequest(Guid id) : IRequest;

    public class DeleteLecturerHandler : IRequestHandler<DeleteLecturerRequest>
    {
        private readonly ILecturerRepository _repo;

        public DeleteLecturerHandler(ILecturerRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteLecturerRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteLecturer(request.id);
        }
    }
}