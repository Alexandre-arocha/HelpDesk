namespace HelpDesk.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? DetalhesAntigos { get; set; }
    public string? DetalhesNovos { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public Guid UsuarioId { get; set; }
}
