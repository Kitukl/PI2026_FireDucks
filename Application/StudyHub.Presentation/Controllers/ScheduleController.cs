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

        public async Task<IActionResult> ScheduleCreate()
        {
            await PrepareGroupsViewBag();
            return View(new ScheduleDto { Lessons = new List<LessonDto>() });
        }

        public IActionResult Create(Guid? groupId)
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
            var schedule = await _mediator.Send(new GetScheduleByGroupIdRequest(id));

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
            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateScheduleRequest(schedule));
                return RedirectToAction(nameof(SchedulesList));
            }
            await PrepareLessonsViewBag();
            return View(schedule);
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