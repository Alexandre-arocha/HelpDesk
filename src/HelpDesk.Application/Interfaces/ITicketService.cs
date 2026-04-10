using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces;

public interface ITicketService
{
    Task<TicketDto> CreateAsync(CreateTicketDto dto, Guid userId);
    Task<TicketDto> GetByIdAsync(Guid id);
    Task<PagedResult<TicketDto>> GetAllAsync(TicketFilterDto filter);
    Task<TicketDto> UpdateAsync(Guid id, UpdateTicketDto dto, Guid userId);
    Task<TicketDto> AssignAsync(Guid ticketId, Guid tecnicoId, Guid userId);
    Task<TicketDto> ChangeStatusAsync(Guid ticketId, ChangeStatusDto dto, Guid userId);
    Task<IEnumerable<TicketDto>> GetByUserAsync(Guid userId);
}
