using MyService.Application.DTOs;

namespace MyService.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> CreateAsync(CreateUserRequest request);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteAsync(Guid id);
}