using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> ChangeRoleAsync(Guid userId, ChangeRoleDto dto);
}
