using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
        });

        // Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Descricao).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Prioridade).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.AnexoUrl).HasMaxLength(500);

            entity.HasOne(e => e.Criador)
                .WithMany(u => u.TicketsCriados)
                .HasForeignKey(e => e.CriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Tecnico)
                .WithMany(u => u.TicketsAtribuidos)
                .HasForeignKey(e => e.TecnicoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Tickets)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Texto).IsRequired();

            entity.HasOne(e => e.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Autor)
                .WithMany(u => u.Comments)
                .HasForeignKey(e => e.AutorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Nome).IsUnique();
            entity.Property(e => e.Descricao).HasMaxLength(500);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Acao).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Ticket)
                .WithMany(t => t.AuditLogs)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var cat1Id = Guid.Parse("00000000-0000-0000-0000-000000000010");
        var cat2Id = Guid.Parse("00000000-0000-0000-0000-000000000011");
        var cat3Id = Guid.Parse("00000000-0000-0000-0000-000000000012");

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            Nome = "Administrador",
            Email = "admin@helpdesk.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            Ativo = true,
            CriadoEm = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = cat1Id, Nome = "Hardware", Descricao = "Problemas com equipamentos fisicos", Ativa = true, CriadoEm = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = cat2Id, Nome = "Software", Descricao = "Problemas com sistemas e aplicativos", Ativa = true, CriadoEm = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = cat3Id, Nome = "Rede", Descricao = "Problemas de conectividade e rede", Ativa = true, CriadoEm = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
