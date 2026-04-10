using AutoMapper;
using FluentAssertions;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Mappings;
using HelpDesk.Application.Services;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Tests.Services;

public class TicketServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly TicketService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public TicketServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new TicketService(_context, _mapper);

        SeedData();
    }

    private void SeedData()
    {
        _context.Users.Add(new User
        {
            Id = _userId,
            Nome = "Test User",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Usuario
        });

        _context.Categories.Add(new Category
        {
            Id = _categoryId,
            Nome = "Hardware"
        });

        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTicketAndAuditLog()
    {
        // Arrange
        var dto = new CreateTicketDto
        {
            Titulo = "PC nao liga",
            Descricao = "Meu computador nao liga desde ontem",
            Prioridade = TicketPriority.Alta,
            CategoriaId = _categoryId
        };

        // Act
        var result = await _sut.CreateAsync(dto, _userId);

        // Assert
        result.Should().NotBeNull();
        result.Titulo.Should().Be("PC nao liga");
        result.Status.Should().Be("Aberto");
        result.CriadorNome.Should().Be("Test User");

        var auditLogs = await _context.AuditLogs.ToListAsync();
        auditLogs.Should().HaveCount(1);
        auditLogs[0].Acao.Should().Be("Chamado criado");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResults()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            _context.Tickets.Add(new Ticket
            {
                Id = Guid.NewGuid(),
                Titulo = $"Ticket {i}",
                Descricao = "Desc",
                CriadorId = _userId,
                CategoriaId = _categoryId
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync(new TicketFilterDto { Page = 1, PageSize = 10 });

        // Assert
        result.TotalCount.Should().Be(15);
        result.Items.Count().Should().Be(10);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task ChangeStatusAsync_ShouldUpdateStatusAndLog()
    {
        // Arrange
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Titulo = "Test",
            Descricao = "Desc",
            CriadorId = _userId,
            CategoriaId = _categoryId,
            Status = TicketStatus.Aberto
        };
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ChangeStatusAsync(ticket.Id, new ChangeStatusDto { Status = TicketStatus.Resolvido }, _userId);

        // Assert
        result.Status.Should().Be("Resolvido");
        var updated = await _context.Tickets.FindAsync(ticket.Id);
        updated!.ResolvidoEm.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
    {
        // Act & Assert
        await _sut.Invoking(s => s.GetByIdAsync(Guid.NewGuid()))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
