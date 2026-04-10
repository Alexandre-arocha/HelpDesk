using System.Security.Claims;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    private Guid GetUserId() => Guid.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<PagedResult<TicketDto>>> GetAll(
        [FromQuery] TicketFilterDto filter)
    {
        var result = await _ticketService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> GetById(Guid id)
    {
        var result = await _ticketService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetMyTickets()
    {
        var result = await _ticketService.GetByUserAsync(GetUserId());
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create([FromBody] CreateTicketDto dto)
    {
        var result = await _ticketService.CreateAsync(dto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TicketDto>> Update(Guid id, [FromBody] UpdateTicketDto dto)
    {
        var result = await _ticketService.UpdateAsync(id, dto, GetUserId());
        return Ok(result);
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Admin,Tecnico")]
    public async Task<ActionResult<TicketDto>> Assign(Guid id, [FromBody] AssignTicketDto dto)
    {
        var result = await _ticketService.AssignAsync(id, dto.TecnicoId, GetUserId());
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Tecnico")]
    public async Task<ActionResult<TicketDto>> ChangeStatus(Guid id, [FromBody] ChangeStatusDto dto)
    {
        var result = await _ticketService.ChangeStatusAsync(id, dto, GetUserId());
        return Ok(result);
    }
}

public class AssignTicketDto
{
    public Guid TecnicoId { get; set; }
}
