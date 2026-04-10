using AutoMapper;
using HelpDesk.Application.DTOs;
using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        CreateMap<Ticket, TicketDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Prioridade, o => o.MapFrom(s => s.Prioridade.ToString()))
            .ForMember(d => d.CriadorNome, o => o.MapFrom(s => s.Criador.Nome))
            .ForMember(d => d.TecnicoNome, o => o.MapFrom(s => s.Tecnico != null ? s.Tecnico.Nome : null))
            .ForMember(d => d.CategoriaNome, o => o.MapFrom(s => s.Categoria.Nome))
            .ForMember(d => d.TotalComentarios, o => o.MapFrom(s => s.Comments.Count));

        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.AutorNome, o => o.MapFrom(s => s.Autor.Nome));

        CreateMap<Category, CategoryDto>();
    }
}
