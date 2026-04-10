using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces;

public interface ICommentService
{
    Task<CommentDto> AddAsync(Guid ticketId, CreateCommentDto dto, Guid userId);
    Task<IEnumerable<CommentDto>> GetByTicketAsync(Guid ticketId);
}
