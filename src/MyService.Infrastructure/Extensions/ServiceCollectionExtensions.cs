using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        // 反射自动注册自定义仓储：Domain.Interfaces 中的接口 → Infrastructure.Repositories 中的实现
        var domainAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .First(a => a.GetName().Name == "MyService.Domain");
        var infraAssembly = typeof(AppDbContext).Assembly;

        var repositoryInterfaces = domainAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"));
        foreach (var interfaceType in repositoryInterfaces)
        {
            var implType = infraAssembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));
            if (implType is not null)
                services.AddScoped(interfaceType, implType);
        }

        // 反射自动注册所有服务：Application.Interfaces 中的接口 → Application.Services 中的实现
        var appAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .First(a => a.GetName().Name == "MyService.Application");

        var serviceInterfaces = appAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Service"));
        foreach (var interfaceType in serviceInterfaces)
        {
            var implType = appAssembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));
            if (implType is not null)
                services.AddScoped(interfaceType, implType);
        }

        return services;
    }
}