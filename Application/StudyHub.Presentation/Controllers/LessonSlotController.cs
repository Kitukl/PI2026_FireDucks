using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.DTOs;
using StudyHub.Core.LessonSlots.Commands;
using StudyHub.Core.LessonSlots.Queries;

namespace StudyHub.Mvc.Controllers
{
    public class LessonSlotController : Controller
    {
        private readonly IMediator _mediator;

        public LessonSlotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> LessonSlotsList()
        {
            var slots = await _mediator.Send(new GetAllLessonSlotsRequest());
            return View(slots);
        }

        public IActionResult LessonSlotCreate()
        {
            return View(new LessonSlotDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LessonSlotCreate(LessonSlotDto lessonSlot)
        {
            ModelState.Remove("Lessons");

            if (!ModelState.IsValid)
            {
                return View(lessonSlot);
            }

            if (ModelState.IsValid)
            {
                await _mediator.Send(new AddLessonSlotRequest(lessonSlot));
                return RedirectToAction(nameof(LessonSlotsList));
            }
            return View(lessonSlot);
        }

        public async Task<IActionResult> LessonSlotEdit(Guid id)
        {
            var slot = await _mediator.Send(new GetLessonSlotByIdRequest(id));
            if (slot == null) return NotFound();
            return View(slot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LessonSlotEdit(Guid id, LessonSlotDto lessonSlot)
        {
            if (id != lessonSlot.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateLessonSlotRequest(lessonSlot));
                return RedirectToAction(nameof(LessonSlotsList));
            }
            return View(lessonSlot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LessonSlotDelete(Guid id)
        {
            await _mediator.Send(new DeleteLessonSlotRequest(id));
            return RedirectToAction(nameof(LessonSlotsList));
        }
    }
}