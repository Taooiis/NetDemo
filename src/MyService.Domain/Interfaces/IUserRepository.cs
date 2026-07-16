using MyService.Domain.Entities;

namespace MyService.Domain.Interfaces;

/// <summary>用户仓储接口（继承通用 CRUD，扩展自定义查询）</summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
}