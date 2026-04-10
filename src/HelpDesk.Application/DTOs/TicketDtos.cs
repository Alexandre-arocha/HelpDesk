using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs;

public class TicketDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Prioridade { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? ResolvidoEm { get; set; }
    public string CriadorNome { get; set; } = string.Empty;
    public string? TecnicoNome { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public string? AnexoUrl { get; set; }
    public int TotalComentarios { get; set; }
}

public class CreateTicketDto
{
    [Required, MaxLength(300)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    public TicketPriority Prioridade { get; set; } = TicketPriority.Media;

    [Required]
    public Guid CategoriaId { get; set; }
}

public class UpdateTicketDto
{
    [MaxLength(300)]
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public TicketPriority? Prioridade { get; set; }
    public Guid? CategoriaId { get; set; }
}

public class ChangeStatusDto
{
    [Required]
    public TicketStatus Status { get; set; }
}

public class TicketFilterDto
{
    public TicketStatus? Status { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? TecnicoId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
