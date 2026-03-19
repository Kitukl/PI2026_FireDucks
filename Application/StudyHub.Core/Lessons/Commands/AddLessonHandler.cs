using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Lessons.Commands
{
    public record AddLessonRequest(LessonDto lesson) : IRequest;

    public class AddLessonSlotHandler : IRequestHandler<AddLessonRequest>
    {
        private readonly ILessonRepository _repo;

        public AddLessonSlotHandler(ILessonRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddLessonRequest request, CancellationToken cancellationToken)
        {
            await _repo.AddLesson(request.lesson);
        }
    }
}