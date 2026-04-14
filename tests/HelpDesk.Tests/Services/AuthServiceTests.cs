using FluentAssertions;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Infrastructure.Services;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Interfaces;
using Moq;

namespace HelpDesk.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IRepository<User>>();
        _tokenServiceMock = new Mock<ITokenService>();
        _sut = new AuthService(_userRepoMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenEmailIsNew()
    {
        // Arrange
        var dto = new RegisterDto { Nome = "Test", Email = "test@test.com", Password = "123456" };
        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<User>());
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);
        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        // Act
        var result = await _sut.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("fake-jwt-token");
        result.Email.Should().Be("test@test.com");
        result.Nome.Should().Be("Test");
        result.Role.Should().Be(UserRole.Usuario.ToString());
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailExists()
    {
        // Arrange
        var dto = new RegisterDto { Nome = "Test", Email = "exists@test.com", Password = "123456" };
        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { new User { Email = "exists@test.com" } });

        // Act & Assert
        await _sut.Invoking(s => s.RegisterAsync(dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ja cadastrado*");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "123456";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nome = "Test",
            Email = "test@test.com",
            PasswordHash = hash,
            Role = UserRole.Tecnico,
            Ativo = true
        };

        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { user });
        _tokenServiceMock.Setup(t => t.GenerateToken(user))
            .Returns("valid-token");

        // Act
        var result = await _sut.LoginAsync(new LoginDto { Email = "test@test.com", Password = password });

        // Assert
        result.Token.Should().Be("valid-token");
        result.Role.Should().Be("Tecnico");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsWrong()
    {
        // Arrange
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
            Ativo = true
        };

        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new[] { user });

        // Act & Assert
        await _sut.Invoking(s => s.LoginAsync(new LoginDto { Email = "test@test.com", Password = "wrong" }))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<User>());

        // Act & Assert
        await _sut.Invoking(s => s.LoginAsync(new LoginDto { Email = "no@user.com", Password = "123" }))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
