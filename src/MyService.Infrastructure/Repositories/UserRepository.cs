using Microsoft.EntityFrameworkCore;
using MyService.Domain.Entities;
using MyService.Domain.Interfaces;
using MyService.Infrastructure.Data;

namespace MyService.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}