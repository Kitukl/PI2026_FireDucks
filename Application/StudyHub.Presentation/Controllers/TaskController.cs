using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.DTOs;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Queries;

namespace Application.Controllers;

[Route("task")]
public class TaskController : Controller
{
    private readonly ISender _mediator;

    public TaskController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var task = await _mediator.Send(new GetTaskQuery
        {
            Id = id
        });

        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _mediator.Send(new GetTasksQuery());
        return View();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateTaskDto request)
    {
        var taskId = await _mediator.Send(new UpdateTaskCommand
        {
            Id = id,
            Subject = request.Subject,
            Status = request.Status
        });
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> GetTask(CreateTaskCommand command)
    {
        var taskId = await _mediator.Send(command);
        return View();
    }
}