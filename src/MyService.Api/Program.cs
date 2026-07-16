using MyService.Api.Middleware;
using MyService.Infrastructure.Data;
using MyService.Infrastructure.Extensions;
using NSwag.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

// 自动创建数据库和表（开发环境）
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();