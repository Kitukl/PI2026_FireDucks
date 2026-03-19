using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Schedule.Interfaces;
using StudyHub.Core.Subjects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Subjects.Commands
{
    public record DeleteSubjectRequest(Guid id) : IRequest;

    public class DeleteSubjectHandler : IRequestHandler<DeleteSubjectRequest>
    {
        private readonly ISubjectRepository _repo;

        public DeleteSubjectHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteSubjectRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteSubject(request.id);
        }
    }
}