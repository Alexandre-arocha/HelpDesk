using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
