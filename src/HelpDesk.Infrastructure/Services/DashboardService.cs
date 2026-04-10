using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetStatsAsync()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Tecnico)
            .Include(t => t.Categoria)
            .ToListAsync();

        var resolvidos = tickets.Where(t => t.ResolvidoEm.HasValue).ToList();
        var tempoMedio = resolvidos.Any()
            ? resolvidos.Average(t => (t.ResolvidoEm!.Value - t.CriadoEm).TotalHours)
            : 0;

        var porTecnico = tickets
            .Where(t => t.Tecnico != null)
            .GroupBy(t => t.Tecnico!.Nome)
            .Select(g => new TecnicoStatsDto
            {
                Nome = g.Key,
                Total = g.Count(),
                Resolvidos = g.Count(t => t.Status == TicketStatus.Resolvido || t.Status == TicketStatus.Fechado)
            })
            .ToList();

        var porCategoria = tickets
            .GroupBy(t => t.Categoria.Nome)
            .Select(g => new CategoriaStatsDto
            {
                Nome = g.Key,
                Total = g.Count()
            })
            .ToList();

        return new DashboardDto
        {
            TotalChamados = tickets.Count,
            ChamadosAbertos = tickets.Count(t => t.Status == TicketStatus.Aberto),
            ChamadosEmAndamento = tickets.Count(t => t.Status == TicketStatus.EmAndamento),
            ChamadosResolvidos = tickets.Count(t => t.Status == TicketStatus.Resolvido),
            ChamadosFechados = tickets.Count(t => t.Status == TicketStatus.Fechado),
            TempoMedioResolucaoHoras = Math.Round(tempoMedio, 2),
            ChamadosPorTecnico = porTecnico,
            ChamadosPorCategoria = porCategoria
        };
    }
}
