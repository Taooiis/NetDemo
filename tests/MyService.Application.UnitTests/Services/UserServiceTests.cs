using Moq;
using MyService.Application.DTOs;
using MyService.Application.Interfaces;
using MyService.Application.Services;
using MyService.Domain.Entities;
using MyService.Domain.Interfaces;

namespace MyService.Application.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _service = new UserService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoUsers()
    {
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(Array.Empty<User>());

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsUsers_WhenUsersExist()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Username = "user1", Email = "user1@test.com", PasswordHash = "hash1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Username = "user2", Email = "user2@test.com", PasswordHash = "hash2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserNotFound()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "testuser", Email = "test@test.com", PasswordHash = "hash", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var result = await _service.GetByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedUser()
    {
        var request = new CreateUserRequest { Username = "newuser", Email = "new@test.com", Password = "password123" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("new@test.com", result.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenUserNotFound()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _service.UpdateAsync(Guid.NewGuid(), new UpdateUserRequest());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedUser_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var existingUser = new User { Id = userId, Username = "oldname", Email = "old@test.com", PasswordHash = "hash", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var updateRequest = new UpdateUserRequest { Username = "newname", Email = "new@test.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(userId, updateRequest);

        Assert.NotNull(result);
        Assert.Equal("newname", result.Username);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenUserNotFound()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenUserDeleted()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "test", Email = "test@test.com", PasswordHash = "hash", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepository.Setup(r => r.DeleteAsync(userId)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(userId);

        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(userId), Times.Once);
    }
}
