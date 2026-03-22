using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Comments.Commands;
using StudyHub.Core.Comments.Queries;
using StudyHub.Core.DTOs;
using StudyHub.Core.Tasks.Commands;
using StudyHub.Core.Tasks.Queries;

namespace Application.Controllers;

[Route("comments")]
public class CommentController : Controller
{
    private readonly ISender _mediator;

    public CommentController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComments(Guid id)
    {
        var comments = await _mediator.Send(new GetCommentsQuery
        {
            TaskId = id
        });

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateComment(CreateCommentCommand command)
    {
        var commentId = await _mediator.Send(command);
        return View();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var commentId = await _mediator.Send(new DeleteCommentCommand
        {
            Id = id
        });
        return View();
    }

}