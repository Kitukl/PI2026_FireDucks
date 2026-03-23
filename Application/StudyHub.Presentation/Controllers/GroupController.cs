using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyHub.Core.DTOs;
using StudyHub.Domain.Entities;
using StudyHub.Infrastructure;

namespace StudyHub.Mvc.Controllers
{
    public class GroupController : Controller
    {
        private readonly SDbContext _context;

        public GroupController(SDbContext context)
        {
            _context = context;
        }

        // Список усіх груп
        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();

            return View(groups);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View(new GroupDto());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupDto groupDto)
        {
            if (ModelState.IsValid)
            {
                var group = new Group
                {
                    Id = Guid.NewGuid(),
                    Name = groupDto.Name
                };

                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groupDto);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var dto = new GroupDto { Id = group.Id, Name = group.Name };
            return View(dto);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GroupDto groupDto)
        {
            if (id != groupDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var group = await _context.Groups.FindAsync(id);
                if (group == null) return NotFound();

                group.Name = groupDto.Name;

                _context.Groups.Update(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groupDto);
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}