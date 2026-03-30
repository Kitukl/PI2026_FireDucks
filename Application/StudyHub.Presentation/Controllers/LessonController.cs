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

        public async Task<IActionResult> LessonsCreate()
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LessonsCreate(LessonDto lesson, Guid[] LecturersIds)
        {
            // Очищаємо валідацію для вкладених об'єктів, які ми вибираємо через Dropdown
            ModelState.Remove("Subject.Name");
            ModelState.Remove("LessonSlot.StartTime");
            ModelState.Remove("LessonSlot.EndTime");

            if (ModelState.IsValid)
            {
                if (LecturersIds != null && LecturersIds.Length > 0)
                {
                    lesson.Lecturers = LecturersIds.Select(id => new LecturerDtoResponse { Id = id }).ToList();
                }

                await _mediator.Send(new AddLessonRequest(lesson));
                return RedirectToAction(nameof(LessonsList)); // Виправлено з Index на LessonsList
            }

            await PrepareViewBags();
            return View(lesson);
        }

        private async Task PrepareViewBags()
        {
            // ВИПРАВЛЕНО: Було GetAllLessonsRequest, тепер GetAllSubjectsRequest
            var subjects = await _mediator.Send(new GetAllSubjectsRequest());
            var slots = await _mediator.Send(new GetAllLessonSlotsRequest());
            var lecturers = await _mediator.Send(new GetAllLecturersRequest());

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
            ViewBag.Slots = new SelectList(slots.Select(s => new {
                Id = s.Id,
                Display = $"{s.StartTime:HH:mm} - {s.EndTime:HH:mm}"
            }), "Id", "Display");

            // Зберігаємо список викладачів для модального вікна
            ViewBag.Lecturers = lecturers;
        }

        public async Task<IActionResult> LessonsEdit(Guid id)
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
        public async Task<IActionResult> LessonsEdit(Guid id, LessonDto lesson)
        {
            //if (id != lesson.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateLessonRequest(lesson));
                return RedirectToAction(nameof(LessonsList));
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
    }
}