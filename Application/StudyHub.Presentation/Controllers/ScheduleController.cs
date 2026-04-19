using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudyHub.Core.DTOs;
using StudyHub.Core.Group.Queries;
using StudyHub.Core.Lessons.Queries;
using StudyHub.Core.Schedules.Commands;
using Task = System.Threading.Tasks.Task;

namespace Application.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly IMediator _mediator;
        public ScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/Schedule")]
        public IActionResult Schedule()
        {
            return RedirectToAction("MySchedule", "Schedule");
        }

        [HttpGet]
        public async Task<IActionResult> MySchedule()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _mediator.Send(new GetScheduleForUserQuery
            {
                UserId = userId
            });

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> SyncMySchedule(string groupName)
        {
            var success = await _mediator.Send(new ParseAndSaveScheduleCommand(groupName));

            if (!success)
            {
                TempData["ErrorMessage"] = "Не вдалося оновити розклад. Перевірте з'єднання з університетським сервером.";
            }
            else
            {
                TempData["SuccessMessage"] = "Розклад успішно оновлено!";
            }

            return RedirectToAction(nameof(MySchedule));
        }

        [HttpGet]
        public async Task<IActionResult> SchedulesList()
        {
            var schedules = await _mediator.Send(new GetAllSchedulesRequest());
            return View(schedules);
        }


        [HttpGet]
        public async Task<IActionResult> ScheduleCreate(Guid? groupId)
        {
            await PrepareGroupsViewBag();

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
        public async Task<IActionResult> ScheduleEdit(Guid id, ScheduleDto schedule)
        {
            schedule.Id = id;

            ModelState.Remove("Group.Name");
            ModelState.Remove("schedule.Group.Name");

            if (schedule.Lessons != null)
            {
                for (int i = 0; i < schedule.Lessons.Count; i++)
                {
                    ModelState.Remove($"Lessons[{i}].Subject.Name");
                    ModelState.Remove($"Lessons[{i}].LessonType");
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

        private async Task ReloadLessonsNames(ScheduleDto scheduleDto)
        {
            if (scheduleDto.Lessons == null) return;
            foreach (var lesson in scheduleDto.Lessons)
            {
                var dbLesson = await _mediator.Send(new GetLessonByIdRequest(lesson.Id));
                if (dbLesson != null)
                {
                    lesson.Subject = new SubjectDto { Name = dbLesson.Subject.Name };
                    lesson.LessonType = dbLesson.LessonType;
                }
            }
        }

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
            var groups = await _mediator.Send(new GetAllGroupsQuery());
            ViewBag.Groups = new SelectList(groups, "Id", "Name");
        }

        private async Task PrepareLessonsViewBag()
        {
            var lessons = await _mediator.Send(new GetAllLessonsRequest());
            ViewBag.Lessons = new SelectList(lessons.Select(l => new {
                Id = l.Id,
                Display = $"{l.Subject.Name} ({l.LessonType}) - Day {l.Day}"
            }), "Id", "Display");
        }
    }
}