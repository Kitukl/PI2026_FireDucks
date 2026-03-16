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

namespace StudyHub.Core.LessonSlots.Commands
{
    public record AddLessonSlotRequest(LessonSlotDto lessonSlot) : IRequest;

    public class AddLessonSlotHandler : IRequestHandler<AddLessonSlotRequest>
    {
        private readonly ILessonSlotRepository _repo;

        public AddLessonSlotHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(AddLessonSlotRequest request, CancellationToken cancellationToken)
        {
            await _repo.AddLessonSlot(request.lessonSlot);
        }
    }
}