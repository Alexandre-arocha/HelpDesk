using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public string AutorNome { get; set; } = string.Empty;
}

public class CreateCommentDto
{
    [Required]
    public string Texto { get; set; } = string.Empty;
}
