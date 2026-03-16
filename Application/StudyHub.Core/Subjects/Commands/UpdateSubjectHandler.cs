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
    public record UpdateSubjectRequest(SubjectDto subject) : IRequest;

    public class UpdateSubjectHandler : IRequestHandler<UpdateSubjectRequest>
    {
        private readonly ISubjectRepository _repo;

        public UpdateSubjectHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateSubjectRequest request, CancellationToken cancellationToken)
        {
            await _repo.UpdateSubject(request.subject);
        }
    }
}