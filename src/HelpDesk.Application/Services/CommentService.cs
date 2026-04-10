using AutoMapper;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Application.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CommentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CommentDto> AddAsync(Guid ticketId, CreateCommentDto dto, Guid userId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket is null)
            throw new KeyNotFoundException("Chamado nao encontrado.");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Texto = dto.Texto,
            TicketId = ticketId,
            AutorId = userId
        };

        _context.Comments.Add(comment);
        ticket.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var saved = await _context.Comments
            .Include(c => c.Autor)
            .FirstAsync(c => c.Id == comment.Id);

        return _mapper.Map<CommentDto>(saved);
    }

    public async Task<IEnumerable<CommentDto>> GetByTicketAsync(Guid ticketId)
    {
        var comments = await _context.Comments
            .Include(c => c.Autor)
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CriadoEm)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
}
