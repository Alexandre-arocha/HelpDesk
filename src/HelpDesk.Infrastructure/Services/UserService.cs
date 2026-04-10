using AutoMapper;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .OrderBy(u => u.Nome)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> ChangeRoleAsync(Guid userId, ChangeRoleDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
            throw new KeyNotFoundException("Usuario nao encontrado.");

        user.Role = dto.Role;
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }
}
