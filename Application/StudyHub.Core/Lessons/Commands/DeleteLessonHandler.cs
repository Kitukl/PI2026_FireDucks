using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Lessons.Commands
{
    public record DeleteLessonRequest(Guid id) : IRequest;

    public class DeleteLessonHandler : IRequestHandler<DeleteLessonRequest>
    {
        private readonly ILessonRepository _repo;

        public DeleteLessonHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteLessonRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteLesson(request.id);
        }
    }
}