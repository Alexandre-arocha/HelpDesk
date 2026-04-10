using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Interfaces;

namespace HelpDesk.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly ITokenService _tokenService;

    public AuthService(IRepository<User> userRepo, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var users = await _userRepo.FindAsync(u => u.Email == dto.Email);
        var user = users.FirstOrDefault();

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email ou senha invalidos.");

        if (!user.Ativo)
            throw new UnauthorizedAccessException("Usuario desativado.");

        return new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(user),
            Email = user.Email,
            Nome = user.Nome,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userRepo.FindAsync(u => u.Email == dto.Email);
        if (existing.Any())
            throw new InvalidOperationException("Email ja cadastrado.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _userRepo.AddAsync(user);

        return new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(user),
            Email = user.Email,
            Nome = user.Nome,
            Role = user.Role.ToString()
        };
    }
}
