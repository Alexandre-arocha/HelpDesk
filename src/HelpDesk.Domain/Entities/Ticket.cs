using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Aberto;
    public TicketPriority Prioridade { get; set; } = TicketPriority.Media;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? ResolvidoEm { get; set; }

    public Guid CriadorId { get; set; }
    public User Criador { get; set; } = null!;

    public Guid? TecnicoId { get; set; }
    public User? Tecnico { get; set; }

    public Guid CategoriaId { get; set; }
    public Category Categoria { get; set; } = null!;

    public string? AnexoUrl { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
