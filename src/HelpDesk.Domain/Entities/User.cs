using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Usuario;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Ticket> TicketsCriados { get; set; } = new List<Ticket>();
    public ICollection<Ticket> TicketsAtribuidos { get; set; } = new List<Ticket>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
