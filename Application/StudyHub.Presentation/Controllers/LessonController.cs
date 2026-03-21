using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lecturers.Queries;
using StudyHub.Core.Lessons.Commands;
using StudyHub.Core.Lessons.Queries;
using StudyHub.Core.LessonSlots.Queries;
using StudyHub.Core.Subjects.Queries;

namespace StudyHub.Mvc.Controllers
{
    public class LessonController : Controller
    {
        private readonly IMediator _mediator;

        public LessonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> LessonsList()
        {
            var lessons = await _mediator.Send(new GetAllLessonsRequest());
            return View(lessons);
        }

        public async Task<IActionResult> LessonDetails(Guid id)
        {
            var lesson = await _mediator.Send(new GetLessonByIdRequest(id));
            if (lesson == null) return NotFound();
            return View(lesson);
        }

        public async Task<IActionResult> LessonCreate()
        {
            await PrepareViewBags();
            return View(new LessonDto());
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LessonCreate(LessonDto lesson)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _mediator.Send(new AddLessonRequest(lesson));
        //        return RedirectToAction(nameof(LessonsList));
        //    }
        //    await PrepareViewBags();
        //    return View(lesson);
        //}

        [HttpPost]
        public async Task<IActionResult> LessonCreate(LessonDto lesson, Guid[] LecturersIds)
        {
            if (LecturersIds != null)
            {
                lesson.Lecturers = LecturersIds.Select(id => new LecturerDtoResponse { Id = id }).ToList();
            }

            await _mediator.Send(new AddLessonRequest(lesson));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var lesson = await _mediator.Send(new GetLessonByIdRequest(id));
            if (lesson == null) return NotFound();

            var subjects = await _mediator.Send(new GetAllSubjectsRequest());
            var slots = await _mediator.Send(new GetAllLessonSlotsRequest());
            var lecturers = await _mediator.Send(new GetAllLecturersRequest());

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name", lesson.Subject.Id);
            ViewBag.Slots = new SelectList(slots.Select(s => new {
                Id = s.Id,
                Display = $"{s.StartTime:HH:mm} - {s.EndTime:HH:mm}"
            }), "Id", "Display", lesson.LessonSlot.Id);

            var selectedLecturersIds = lesson.Lecturers.Select(l => l.Id).ToArray();
            ViewBag.Lecturers = new MultiSelectList(lecturers.Select(l => new {
                Id = l.Id,
                FullName = $"{l.Surname} {l.Name}"
            }), "Id", "FullName", selectedLecturersIds);

            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LessonEdit(Guid id, LessonDto lesson)
        {
            if (id != lesson.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateLessonRequest(lesson));
                return RedirectToAction(nameof(Index));
            }
            await PrepareViewBags();
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LessonDelete(Guid id)
        {
            await _mediator.Send(new DeleteLessonRequest(id));
            return RedirectToAction(nameof(LessonsList));
        }

        private async Task PrepareViewBags()
        {
            var subjects = await _mediator.Send(new GetAllLessonsRequest());
            var slots = await _mediator.Send(new GetAllLessonSlotsRequest());
            var lecturers = await _mediator.Send(new GetAllLecturersRequest());

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
            ViewBag.Slots = new SelectList(slots.Select(s => new {
                Id = s.Id,
                Display = $"{s.StartTime:HH:mm} - {s.EndTime:HH:mm}"
            }), "Id", "Display");

            ViewBag.Lecturers = new MultiSelectList(lecturers.Select(l => new {
                Id = l.Id,
                FullName = $"{l.Surname} {l.Name}"
            }), "Id", "FullName");
        }
    }
}