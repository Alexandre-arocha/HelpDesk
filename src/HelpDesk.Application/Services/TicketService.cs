using AutoMapper;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Interfaces;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Application.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TicketService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TicketDto> CreateAsync(CreateTicketDto dto, Guid userId)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Prioridade = dto.Prioridade,
            CategoriaId = dto.CategoriaId,
            CriadorId = userId
        };

        _context.Tickets.Add(ticket);

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            Acao = "Chamado criado",
            DetalhesNovos = $"Titulo: {ticket.Titulo}",
            TicketId = ticket.Id,
            UsuarioId = userId
        });

        await _context.SaveChangesAsync();

        return await GetByIdAsync(ticket.Id);
    }

    public async Task<TicketDto> GetByIdAsync(Guid id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Criador)
            .Include(t => t.Tecnico)
            .Include(t => t.Categoria)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket is null)
            throw new KeyNotFoundException("Chamado nao encontrado.");

        return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<PagedResult<TicketDto>> GetAllAsync(TicketFilterDto filter)
    {
        var query = _context.Tickets
            .Include(t => t.Criador)
            .Include(t => t.Tecnico)
            .Include(t => t.Categoria)
            .Include(t => t.Comments)
            .AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);
        if (filter.CategoriaId.HasValue)
            query = query.Where(t => t.CategoriaId == filter.CategoriaId.Value);
        if (filter.TecnicoId.HasValue)
            query = query.Where(t => t.TecnicoId == filter.TecnicoId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CriadoEm)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<TicketDto>
        {
            Items = _mapper.Map<IEnumerable<TicketDto>>(items),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<TicketDto> UpdateAsync(Guid id, UpdateTicketDto dto, Guid userId)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket is null)
            throw new KeyNotFoundException("Chamado nao encontrado.");

        var oldDetails = $"Titulo: {ticket.Titulo}, Prioridade: {ticket.Prioridade}";

        if (dto.Titulo is not null) ticket.Titulo = dto.Titulo;
        if (dto.Descricao is not null) ticket.Descricao = dto.Descricao;
        if (dto.Prioridade.HasValue) ticket.Prioridade = dto.Prioridade.Value;
        if (dto.CategoriaId.HasValue) ticket.CategoriaId = dto.CategoriaId.Value;
        ticket.AtualizadoEm = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            Acao = "Chamado atualizado",
            DetalhesAntigos = oldDetails,
            DetalhesNovos = $"Titulo: {ticket.Titulo}, Prioridade: {ticket.Prioridade}",
            TicketId = ticket.Id,
            UsuarioId = userId
        });

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<TicketDto> AssignAsync(Guid ticketId, Guid tecnicoId, Guid userId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket is null)
            throw new KeyNotFoundException("Chamado nao encontrado.");

        ticket.TecnicoId = tecnicoId;
        ticket.Status = TicketStatus.EmAndamento;
        ticket.AtualizadoEm = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            Acao = "Chamado atribuido a tecnico",
            DetalhesNovos = $"TecnicoId: {tecnicoId}",
            TicketId = ticket.Id,
            UsuarioId = userId
        });

        await _context.SaveChangesAsync();
        return await GetByIdAsync(ticketId);
    }

    public async Task<TicketDto> ChangeStatusAsync(Guid ticketId, ChangeStatusDto dto, Guid userId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket is null)
            throw new KeyNotFoundException("Chamado nao encontrado.");

        var oldStatus = ticket.Status;
        ticket.Status = dto.Status;
        ticket.AtualizadoEm = DateTime.UtcNow;

        if (dto.Status == TicketStatus.Resolvido)
            ticket.ResolvidoEm = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            Acao = "Status alterado",
            DetalhesAntigos = oldStatus.ToString(),
            DetalhesNovos = dto.Status.ToString(),
            TicketId = ticket.Id,
            UsuarioId = userId
        });

        await _context.SaveChangesAsync();
        return await GetByIdAsync(ticketId);
    }

    public async Task<IEnumerable<TicketDto>> GetByUserAsync(Guid userId)
    {
        var tickets = await _context.Tickets
            .Include(t => t.Criador)
            .Include(t => t.Tecnico)
            .Include(t => t.Categoria)
            .Include(t => t.Comments)
            .Where(t => t.CriadorId == userId)
            .OrderByDescending(t => t.CriadoEm)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }
}
