namespace HelpDesk.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public Guid AutorId { get; set; }
    public User Autor { get; set; } = null!;
}
