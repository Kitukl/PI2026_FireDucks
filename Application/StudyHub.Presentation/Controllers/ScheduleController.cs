using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Queries;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Queries;
using StudyHub.Infrastructure;

namespace StudyHub.Mvc.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SDbContext _context;

        public ScheduleController(IMediator mediator, SDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<IActionResult> SchedulesList()
        {
            var schedules = await _mediator.Send(new GetAllSchedulesRequest());
            return View(schedules);
        }



        public IActionResult ScheduleCreate(Guid? groupId)
        {
            var model = new ScheduleDto
            {
                Group = new GroupDto { Id = groupId ?? Guid.Empty },
                Lessons = new List<LessonDto>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleCreate(ScheduleDto schedule)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(new AddScheduleRequest(schedule));
                return RedirectToAction(nameof(SchedulesList));
            }
            await PrepareGroupsViewBag();
            return View(schedule);
        }

        public async Task<IActionResult> ScheduleEdit(Guid id)
        {
            var schedule = await _mediator.Send(new GetScheduleByIdRequest(id));
            if (schedule == null) return NotFound();

            await PrepareLessonsViewBag();
            return View(schedule);
        }

        public async Task<IActionResult> ScheduleDetails(Guid id)
        {
            var schedule = await _mediator.Send(new GetScheduleByIdRequest(id));

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleEdit(Guid id, ScheduleDto schedule)
        {
            // ДОДАЙ ОСЬ ЦЕЙ РЯДОК:
            schedule.Id = id; // Примусово беремо ID з URL і кладемо в об'єкт

            // Далі твій існуючий код:
            ModelState.Remove("Group.Name");
            ModelState.Remove("schedule.Group.Name");

            if (schedule.Lessons != null)
            {
                for (int i = 0; i < schedule.Lessons.Count; i++)
                {
                    ModelState.Remove($"schedule.Lessons[{i}].Subject.Name");
                    ModelState.Remove($"schedule.Lessons[{i}].LessonType");
                }
            }

            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateScheduleRequest(schedule));
                return RedirectToAction(nameof(SchedulesList));
            }

            await ReloadLessonsNames(schedule);
            await PrepareLessonsViewBag();
            return View(schedule);
        }

        private async Task ReloadLessonsNames(ScheduleDto dto)
        {
            if (dto.Lessons == null) return;
            foreach (var lesson in dto.Lessons)
            {
                var dbLesson = await _context.Lessons
                    .Include(l => l.Subject)
                    .FirstOrDefaultAsync(l => l.Id == lesson.Id);
                if (dbLesson != null)
                {
                    lesson.Subject = new SubjectDto { Name = dbLesson.Subject.Name };
                    lesson.LessonType = dbLesson.LessonType;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleDelete(Guid id)
        {
            var schedule = await _mediator.Send(new GetScheduleByIdRequest(id));

            if (schedule != null && schedule.Group != null)
            {
                await _mediator.Send(new DeleteScheduleForGroupRequest(schedule.Group.Id));
            }

            return RedirectToAction(nameof(SchedulesList));
        }

        private async Task PrepareGroupsViewBag()
        {
            var groups = await _context.Groups
                .Select(g => new { g.Id, g.Name })
                .ToListAsync();

            ViewBag.Groups = new SelectList(groups, "Id", "Name");
        }

        private async Task PrepareLessonsViewBag()
        {
            var lessons = await _mediator.Send(new GetAllLessonsRequest());
            ViewBag.Lessons = new SelectList(lessons.Select(l => new {
                Id = l.Id,
                Display = $"{l.Subject.Name} ({l.LessonType}) - {l.Day} день"
            }), "Id", "Display");
        }
    }
}