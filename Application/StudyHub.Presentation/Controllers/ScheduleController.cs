using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Core.Lessons.Queries;
using StudyHub.Core.Schedules.Commands;
using StudyHub.Core.Schedules.Queries;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Domain.Enums;
using StudyHub.Infrastructure;
using DayOfWeek = StudyHub.Domain.Enums.DayOfWeek;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Mvc.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepo;

        public ScheduleController(IMediator mediator, SDbContext context, UserManager<User> userManager, IUserRepository userRepo)
        {
            _mediator = mediator;
            _context = context;
            _userManager = userManager;
            _userRepo = userRepo;
        }


        [HttpGet]
        public async Task<IActionResult> MySchedule()
        {
            var userIdString = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "User");

            var users = await _userRepo.GetUsersAsync();
            var currentUser = users.FirstOrDefault(u => u.Id == Guid.Parse(userIdString));

            if (currentUser?.Group == null)
            {
                return View("NoGroupAssigned");
            }

            var schedule = await _mediator.Send(new GetScheduleByGroupIdRequest(currentUser.Group.Id));

            var isHeadman = await _userManager.IsInRoleAsync(currentUser, Role.Leader.ToString());

            var vm = new ScheduleViewModel
            {
                GroupId = currentUser.Group.Id,
                GroupName = currentUser.Group.Name,
                IsHeadman = isHeadman,
                CanHeadmanUpdate = schedule?.HeadmanUpdate ?? false
            };

            if (schedule != null && schedule.Lessons.Any())
            {
                vm.UniqueSlots = schedule.Lessons
                    .Select(l => l.LessonSlot)
                    .GroupBy(s => s.Id)
                    .Select(g => g.First())
                    .OrderBy(s => s.StartTime)
                    .ToList();

                vm.Days = new List<DayOfWeek> {
                    DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                    DayOfWeek.Thursday, DayOfWeek.Friday
                };

                foreach (var lesson in schedule.Lessons)
                {
                    var key = $"{(int)lesson.Day}-{lesson.LessonSlot.Id}";
                    if (!vm.Grid.ContainsKey(key))
                    {
                        vm.Grid[key] = new List<LessonDto>();
                    }
                    vm.Grid[key].Add(lesson);
                }
            }

            return View(vm);
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

        public async Task<IActionResult> ScheduleDelete(Guid id)
        {
            var schedule = await _mediator.Send(new GetScheduleByIdRequest(id));

            if (schedule != null && schedule.Group != null)
            {
                await _mediator.Send(new DeleteScheduleForGroupRequest(schedule.Group.Id));
            }

            return RedirectToAction(nameof(SchedulesList));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ScheduleDelete(Guid id)
        //{
        //    var schedule = await _mediator.Send(new GetScheduleByIdRequest(id));

        //    if (schedule != null && schedule.Group != null)
        //    {
        //        await _mediator.Send(new DeleteScheduleForGroupRequest(schedule.Group.Id));
        //    }

        //    return RedirectToAction(nameof(SchedulesList));
        //}

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