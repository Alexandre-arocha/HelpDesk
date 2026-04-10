namespace HelpDesk.Application.DTOs;

public class DashboardDto
{
    public int TotalChamados { get; set; }
    public int ChamadosAbertos { get; set; }
    public int ChamadosEmAndamento { get; set; }
    public int ChamadosResolvidos { get; set; }
    public int ChamadosFechados { get; set; }
    public double TempoMedioResolucaoHoras { get; set; }
    public IEnumerable<TecnicoStatsDto> ChamadosPorTecnico { get; set; } = Enumerable.Empty<TecnicoStatsDto>();
    public IEnumerable<CategoriaStatsDto> ChamadosPorCategoria { get; set; } = Enumerable.Empty<CategoriaStatsDto>();
}

public class TecnicoStatsDto
{
    public string Nome { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Resolvidos { get; set; }
}

public class CategoriaStatsDto
{
    public string Nome { get; set; } = string.Empty;
    public int Total { get; set; }
}
