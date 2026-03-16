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
    public record AddSubjectRequest(SubjectDto subject) : IRequest;

    public class AddSubjectHandler : IRequestHandler<AddSubjectRequest>
    {
        private readonly ISubjectRepository _repo;

        public AddSubjectHandler(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddSubjectRequest request, CancellationToken cancellationToken)
        {
            await _repo.AddSubject(request.subject);
        }
    }
}