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
    public record UpdateLessonSlotRequest(LessonSlotDto lessonSlot) : IRequest;

    public class UpdateLessonSlotHandler : IRequestHandler<UpdateLessonSlotRequest>
    {
        private readonly ILessonSlotRepository _repo;

        public UpdateLessonSlotHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateLessonSlotRequest request, CancellationToken cancellationToken)
        {
            await _repo.UpdateLessonSlot(request.lessonSlot);
        }
    }
}