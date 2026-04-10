using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class ChangeRoleDto
{
    [Required]
    public UserRole Role { get; set; }
}
