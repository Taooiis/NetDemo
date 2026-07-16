# MyService 微服务实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 创建一个基于 EF Core Code First 的 DDD 微服务，包含完整的用户管理功能、Swagger 文档和单元测试。

**Architecture:** DDD 分层架构（Api → Application → Domain，Infrastructure 作为支持层），使用仓储模式，EF Core 管理数据库迁移。

**Tech Stack:** .NET 8, C# 12, EF Core 8, MySQL, NSwag, xUnit

---

## 文件结构

```
MyService.sln
├── src/
│   ├── MyService.Api/
│   │   ├── Controllers/UsersController.cs
│   │   ├── Middleware/ExceptionHandlingMiddleware.cs
│   │   ├── Extensions/ServiceCollectionExtensions.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── MyService.Domain/
│   │   ├── Entities/User.cs
│   │   ├── Interfaces/IUserRepository.cs
│   │   └── Events/DomainEvents.cs
│   ├── MyService.Application/
│   │   ├── DTOs/UserDto.cs
│   │   ├── DTOs/CreateUserRequest.cs
│   │   ├── DTOs/UpdateUserRequest.cs
│   │   ├── Interfaces/IUserService.cs
│   │   └── Services/UserService.cs
│   ├── MyService.Infrastructure/
│   │   ├── Data/AppDbContext.cs
│   │   ├── Data/Configurations/UserConfiguration.cs
│   │   ├── Repositories/UserRepository.cs
│   │   └── Extensions/ServiceCollectionExtensions.cs
│   └── MyService.Common/
│       ├── Extensions/StringExtensions.cs
│       └── Helpers/Result.cs
└── tests/
    ├── MyService.Domain.UnitTests/
    └── MyService.Application.UnitTests/
```

---

## Task 1: 创建解决方案和项目结构

**Files:**
- Create: `MyService.sln`
- Create: `src/MyService.Api/MyService.Api.csproj`
- Create: `src/MyService.Domain/MyService.Domain.csproj`
- Create: `src/MyService.Application/MyService.Application.csproj`
- Create: `src/MyService.Infrastructure/MyService.Infrastructure.csproj`
- Create: `src/MyService.Common/MyService.Common.csproj`
- Create: `tests/MyService.Domain.UnitTests/MyService.Domain.UnitTests.csproj`
- Create: `tests/MyService.Application.UnitTests/MyService.Application.UnitTests.csproj`

- [ ] **Step 1: 创建解决方案文件**

```bash
cd D:\Work\NetDemo
dotnet new sln -n MyService
```

- [ ] **Step 2: 创建所有项目**

```bash
dotnet new classlib -n MyService.Common -o src/MyService.Common -f net8.0
dotnet new classlib -n MyService.Domain -o src/MyService.Domain -f net8.0
dotnet new classlib -n MyService.Application -o src/MyService.Application -f net8.0
dotnet new classlib -n MyService.Infrastructure -o src/MyService.Infrastructure -f net8.0
dotnet new webapi -n MyService.Api -o src/MyService.Api -f net8.0 --use-controllers
dotnet new xunit -n MyService.Domain.UnitTests -o tests/MyService.Domain.UnitTests -f net8.0
dotnet new xunit -n MyService.Application.UnitTests -o tests/MyService.Application.UnitTests -f net8.0
```

- [ ] **Step 3: 添加项目引用**

```bash
dotnet sln MyService.sln add src/MyService.Api/MyService.Api.csproj
dotnet sln MyService.sln add src/MyService.Domain/MyService.Domain.csproj
dotnet sln MyService.sln add src/MyService.Application/MyService.Application.csproj
dotnet sln MyService.sln add src/MyService.Infrastructure/MyService.Infrastructure.csproj
dotnet sln MyService.sln add src/MyService.Common/MyService.Common.csproj
dotnet sln MyService.sln add tests/MyService.Domain.UnitTests/MyService.Domain.UnitTests.csproj
dotnet sln MyService.sln add tests/MyService.Application.UnitTests/MyService.Application.UnitTests.csproj
```

- [ ] **Step 4: 配置项目间引用**

```bash
dotnet add src/MyService.Application/MyService.Application.csproj reference src/MyService.Domain/MyService.Domain.csproj
dotnet add src/MyService.Infrastructure/MyService.Infrastructure.csproj reference src/MyService.Domain/MyService.Domain.csproj
dotnet add src/MyService.Infrastructure/MyService.Infrastructure.csproj reference src/MyService.Application/MyService.Application.csproj
dotnet add src/MyService.Api/MyService.Api.csproj reference src/MyService.Application/MyService.Application.csproj
dotnet add src/MyService.Api/MyService.Api.csproj reference src/MyService.Infrastructure/MyService.Infrastructure.csproj
dotnet add src/MyService.Api/MyService.Api.csproj reference src/MyService.Common/MyService.Common.csproj
dotnet add tests/MyService.Application.UnitTests/MyService.Application.UnitTests.csproj reference src/MyService.Application/MyService.Application.csproj
```

- [ ] **Step 5: 添加 NuGet 包**

```bash
# Infrastructure - EF Core + MySQL
dotnet add src/MyService.Infrastructure/MyService.Infrastructure.csproj package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add src/MyService.Infrastructure/MyService.Infrastructure.csproj package Pomelo.EntityFrameworkCore.MySql --version 8.0.0
dotnet add src/MyService.Infrastructure/MyService.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.0

# Api - Swagger
dotnet add src/MyService.Api/MyService.Api.csproj package NSwag.AspNetCore --version 14.0.0

# Tests - Mocking
dotnet add tests/MyService.Application.UnitTests/MyService.Application.UnitTests.csproj package Moq --version 4.20.0
```

- [ ] **Step 6: 删除默认 Class1.cs 文件**

```bash
Remove-Item src/MyService.Domain/Class1.cs
Remove-Item src/MyService.Application/Class1.cs
Remove-Item src/MyService.Infrastructure/Class1.cs
Remove-Item src/MyService.Common/Class1.cs
```

- [ ] **Step 7: 验证解决方案构建**

```bash
dotnet build MyService.sln
```

---

## Task 2: 实现 Domain 层

**Files:**
- Create: `src/MyService.Domain/Entities/User.cs`
- Create: `src/MyService.Domain/Interfaces/IUserRepository.cs`
- Create: `src/MyService.Domain/Common/BaseEntity.cs`
- Create: `src/MyService.Domain/Common/Entity.cs`

- [ ] **Step 1: 创建 BaseEntity 基类**

```csharp
namespace MyService.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

- [ ] **Step 2: 创建 User 实体**

```csharp
using MyService.Domain.Common;

namespace MyService.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
```

- [ ] **Step 3: 创建 IUserRepository 接口**

```csharp
using MyService.Domain.Entities;

namespace MyService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}
```

- [ ] **Step 4: 验证 Domain 层构建**

```bash
dotnet build src/MyService.Domain/MyService.Domain.csproj
```

---

## Task 3: 实现 Application 层

**Files:**
- Create: `src/MyService.Application/DTOs/UserDto.cs`
- Create: `src/MyService.Application/DTOs/CreateUserRequest.cs`
- Create: `src/MyService.Application/DTOs/UpdateUserRequest.cs`
- Create: `src/MyService.Application/Interfaces/IUserService.cs`
- Create: `src/MyService.Application/Services/UserService.cs`

- [ ] **Step 1: 创建 UserDto**

```csharp
namespace MyService.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

- [ ] **Step 2: 创建 CreateUserRequest**

```csharp
namespace MyService.Application.DTOs;

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

- [ ] **Step 3: 创建 UpdateUserRequest**

```csharp
namespace MyService.Application.DTOs;

public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

- [ ] **Step 4: 创建 IUserService 接口**

```csharp
using MyService.Application.DTOs;

namespace MyService.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteAsync(Guid id);
}
```

- [ ] **Step 5: 创建 UserService 实现**

```csharp
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

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCryptHash(request.Password),
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

    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password)));
}
```

- [ ] **Step 6: 验证 Application 层构建**

```bash
dotnet build src/MyService.Application/MyService.Application.csproj
```

---

## Task 4: 实现 Infrastructure 层

**Files:**
- Create: `src/MyService.Infrastructure/Data/AppDbContext.cs`
- Create: `src/MyService.Infrastructure/Data/Configurations/UserConfiguration.cs`
- Create: `src/MyService.Infrastructure/Repositories/UserRepository.cs`
- Create: `src/MyService.Infrastructure/Extensions/ServiceCollectionExtensions.cs`

- [ ] **Step 1: 创建 AppDbContext**

```csharp
using Microsoft.EntityFrameworkCore;
using MyService.Domain.Entities;

namespace MyService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

- [ ] **Step 2: 创建 UserConfiguration**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyService.Domain.Entities;

namespace MyService.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(256)
            .IsRequired();
    }
}
```

- [ ] **Step 3: 创建 UserRepository 实现**

```csharp
using Microsoft.EntityFrameworkCore;
using MyService.Domain.Entities;
using MyService.Domain.Interfaces;
using MyService.Infrastructure.Data;

namespace MyService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user is not null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
```

- [ ] **Step 4: 创建 ServiceCollectionExtensions**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyService.Application.Interfaces;
using MyService.Application.Services;
using MyService.Domain.Interfaces;
using MyService.Infrastructure.Data;
using MyService.Infrastructure.Repositories;

namespace MyService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
```

- [ ] **Step 5: 验证 Infrastructure 层构建**

```bash
dotnet build src/MyService.Infrastructure/MyService.Infrastructure.csproj
```

---

## Task 5: 实现 Api 层

**Files:**
- Create: `src/MyService.Api/Controllers/UsersController.cs`
- Create: `src/MyService.Api/Middleware/ExceptionHandlingMiddleware.cs`
- Modify: `src/MyService.Api/Program.cs`
- Modify: `src/MyService.Api/appsettings.json`

- [ ] **Step 1: 创建 ExceptionHandlingMiddleware**

```csharp
using System.Net;
using System.Text.Json;

namespace MyService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = "An error occurred while processing your request.",
            detail = exception.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

- [ ] **Step 2: 创建 UsersController**

```csharp
using Microsoft.AspNetCore.Mvc;
using MyService.Application.DTOs;
using MyService.Application.Interfaces;

namespace MyService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(id, request);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
```

- [ ] **Step 3: 修改 appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyServiceDb;User=root;Password=yourpassword;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

- [ ] **Step 4: 修改 Program.cs**

```csharp
using MyService.Api.Middleware;
using MyService.Infrastructure.Extensions;
using NSwag.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));

builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "MyService API";
    settings.Version = "v1";
    settings.Description = "A DDD-based microservice with EF Core Code First";
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

- [ ] **Step 5: 删除默认生成的 WeatherForecastController.cs 和 Files**

```bash
Remove-Item src/MyService.Api/Controllers/WeatherForecastController.cs
Remove-Item src/MyService.Api/WeatherForecast.cs
```

- [ ] **Step 6: 验证 Api 层构建**

```bash
dotnet build src/MyService.Api/MyService.Api.csproj
```

---

## Task 6: 创建单元测试

**Files:**
- Create: `tests/MyService.Application.UnitTests/Services/UserServiceTests.cs`

- [ ] **Step 1: 创建 UserServiceTests**

```csharp
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
```

- [ ] **Step 2: 验证测试构建**

```bash
dotnet build tests/MyService.Application.UnitTests/MyService.Application.UnitTests.csproj
```

- [ ] **Step 3: 运行单元测试**

```bash
dotnet test tests/MyService.Application.UnitTests/MyService.Application.UnitTests.csproj --verbosity normal
```

---

## Task 7: 验证完整构建

- [ ] **Step 1: 清理并构建整个解决方案**

```bash
dotnet clean MyService.sln
dotnet build MyService.sln
```

- [ ] **Step 2: 运行所有测试**

```bash
dotnet test MyService.sln --verbosity normal
```

---

## 自检清单

- [ ] 所有项目引用正确配置
- [ ] EF Core 实体配置正确（Username、Email 唯一索引）
- [ ] Swagger 文档可访问（/swagger）
- [ ] 所有测试通过
- [ ] 解决方案成功构建，无警告

---

**Plan complete and saved to `docs/superpowers/plans/2026-07-14-myservice-implementation-plan.md`**

**两个执行选项：**

**1. Subagent-Driven (recommended)** - 每个 Task 由独立 subagent 执行，任务间有审查

**2. Inline Execution** - 在当前 session 中按批次执行任务，带检查点

**选择哪个方式？**