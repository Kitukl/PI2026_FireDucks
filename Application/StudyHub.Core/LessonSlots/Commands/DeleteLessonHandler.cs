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
    public record DeleteLessonSlotRequest(Guid id) : IRequest;

    public class DeleteLessonSlotHandler : IRequestHandler<DeleteLessonSlotRequest>
    {
        private readonly ILessonSlotRepository _repo;

        public DeleteLessonSlotHandler(ILessonSlotRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteLessonSlotRequest request, CancellationToken cancellationToken)
        {
            await _repo.DeleteLessonSlot(request.id);
        }
    }
}