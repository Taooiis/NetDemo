using MyService.Application.DTOs;
using MyService.Application.Interfaces;
using MyService.Domain.Entities;
using MyService.Domain.Interfaces;

namespace MyService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto?> CreateAsync(CreateUserRequest request)
    {
        var existing = await _userRepository.GetByUsernameAsync(request.Username)
            ?? await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null) return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return null;

        var duplicate = await _userRepository.GetByUsernameAsync(request.Username);
        if (duplicate is not null && duplicate.Id != id) return null;
        duplicate = await _userRepository.GetByEmailAsync(request.Email);
        if (duplicate is not null && duplicate.Id != id) return null;

        user.Username = request.Username;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };

    private static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);
}