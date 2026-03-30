using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.DTOs;
using StudyHub.Core.Subjects.Commands;
using StudyHub.Core.Subjects.Queries;

namespace StudyHub.Mvc.Controllers
{
    public class SubjectController : Controller
    {
        private readonly IMediator _mediator;

        public SubjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> SubjectsList()
        {
            var subjects = await _mediator.Send(new GetAllSubjectsRequest());
            return View(subjects);
        }

        public IActionResult SubjectCreate()
        {
            return View(new SubjectDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectCreate(SubjectDto subject)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(new AddSubjectRequest(subject));
                return RedirectToAction(nameof(SubjectsList));
            }
            return View(subject);
        }

        public async Task<IActionResult> SubjectEdit(Guid id)
        {
            var subject = await _mediator.Send(new GetSubjectByIdRequest(id));
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectEdit(Guid id, SubjectDto subject)
        {
            if (id != subject.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(new UpdateSubjectRequest(subject));
                return RedirectToAction(nameof(SubjectsList));
            }
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectDelete(Guid id)
        {
            await _mediator.Send(new DeleteSubjectRequest(id));
            return RedirectToAction(nameof(SubjectsList));
        }
    }
}