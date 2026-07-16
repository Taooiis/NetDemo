using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyService.Application.Interfaces;
using MyService.Domain.Interfaces;
using MyService.Infrastructure.Data;
using MyService.Infrastructure.Repositories;

namespace MyService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(connectionString));

        // 注册通用仓储（新增简单实体无需建接口和实现，直接注入 IRepository<T>）
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // 使用 Scrutor 批量注册自定义仓储（排除通用 Repository<T> 基类）
        services.Scan(scan => scan
            .FromAssemblies(typeof(UserRepository).Assembly)
            .AddClasses(classes => classes.Where(t => t is { IsGenericTypeDefinition: false, Name: { } name }
                && name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // 使用 Scrutor 批量注册服务：Application.Interfaces → Application.Services
        services.Scan(scan => scan
            .FromAssemblies(typeof(IUserService).Assembly)
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}