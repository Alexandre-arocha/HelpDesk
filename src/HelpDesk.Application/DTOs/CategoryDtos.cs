using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativa { get; set; }
}

public class CreateCategoryDto
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descricao { get; set; }
}
