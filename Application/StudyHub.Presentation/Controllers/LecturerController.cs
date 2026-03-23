using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Commands;
using StudyHub.Core.Lecturers.Queries;
using StudyHub.Core.LessonSlots.Queries;
using StudyHub.Core.Subjects.Queries;

namespace StudyHub.Mvc.Controllers
{
    public class LecturerController : Controller
    {
        private readonly IMediator _mediator;

        public LecturerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> LecturersList()
        {
            var result = await _mediator.Send(new GetAllLecturersRequest());
            return View(result);
        }

        public async Task<IActionResult> LecturerDetails(Guid id)
        {
            var result = await _mediator.Send(new GetLecturerByIdRequest(id));
            if (result == null) return NotFound();
            return View(result);
        }

        public async Task<IActionResult> LecturerCreate()
        {
            await PrepareViewBags();
            return View(new LecturerDtoRequest { Lessons = new List<LessonDto>() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LecturerCreate(LecturerDtoRequest lecturer)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                await _mediator.Send(new AddLecturerRequest(lecturer));
                return RedirectToAction(nameof(LecturersList));
            }
            await PrepareViewBags();
            return View(lecturer);
        }
        public async Task<IActionResult> LecturerEdit(Guid id)
        {
            var lecturer = await _mediator.Send(new GetLecturerByIdRequest(id));
            if (lecturer == null) return NotFound();

            var request = new LecturerDtoRequest
            {
                Id = lecturer.Id,
                Name = lecturer.Name,
                Surname = lecturer.Surname
            };

            await PrepareViewBags();
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LecturerEdit(Guid id, LecturerDtoRequest lecturer)
        {
            ModelState.Clear();

            if (id != lecturer.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateLecturerRequest(lecturer));
                return RedirectToAction(nameof(LecturersList));
            }
            await PrepareViewBags();
            return View(lecturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LecturerDelete(Guid id)
        {
            await _mediator.Send(new DeleteLecturerRequest(id));
            return RedirectToAction(nameof(LecturersList));
        }

        private async Task PrepareViewBags()
        {
            var subjects = await _mediator.Send(new GetAllSubjectsRequest());
            var slots = await _mediator.Send(new GetAllLessonSlotsRequest());

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
            ViewBag.Slots = new SelectList(slots.Select(s => new {
                Id = s.Id,
                Display = $"{s.StartTime:HH:mm} - {s.EndTime:HH:mm}"
            }), "Id", "Display");
        }
    }
}