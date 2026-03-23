using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Feedbacks.Commands;
using StudyHub.Core.Feedbacks.Queries;
using StudyHub.Domain.Enums;

namespace Application.Controllers;

[Route("task")]
public class FeedbackController : Controller
{
    private readonly ISender _mediator;

    public FeedbackController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFeedback(Guid id)
    {
        var feedback = await _mediator.Send(new GetFeedbackCommand
        {
            Id = id
        });

        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFeedbacks()
    {
        var feedbacks = await _mediator.Send(new GetFeedbacksCommand());
        return View();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] Status status)
    {
        var feedbackId = await _mediator.Send(new UpdateFeedbackCommand
        {
            Id = id,
            Status = status
        });
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateFeedbackCommand command)
    {
        var feedbackId = await _mediator.Send(command);
        return View();
    }
}