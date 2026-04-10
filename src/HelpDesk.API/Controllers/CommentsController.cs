using System.Security.Claims;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/tickets/{ticketId}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    private Guid GetUserId() => Guid.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetByTicket(Guid ticketId)
    {
        var result = await _commentService.GetByTicketAsync(ticketId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Add(Guid ticketId, [FromBody] CreateCommentDto dto)
    {
        var result = await _commentService.AddAsync(ticketId, dto, GetUserId());
        return Created("", result);
    }
}
