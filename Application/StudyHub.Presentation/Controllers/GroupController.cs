using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Group.Commands;
using StudyHub.Core.Group.Queries;
using StudyHub.Domain.Enums;

namespace Application.Controllers
{
    [Authorize(Roles = nameof(Role.Admin))]
    public class GroupController : Controller
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var groups = await _mediator.Send(new GetAllGroupsQuery());
            return View(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupCommand request)
        {
            var response = await _mediator.Send(request);
            return View(response);
        }

        // GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(GetGroupQuery request)
        {
            var response = await _mediator.Send(request);
            return View(response);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateGroupCommand request)
        {
            var response = await _mediator.Send(request);
            if (!response)
            {
                return BadRequest();
            }
            return View(request);
        }

        // POST: Delete
        [HttpPost]
        public async Task<IActionResult> Delete(DeleteGroupCommand request)
        {
            var isDeleted = await _mediator.Send(request);
            if (!isDeleted)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
