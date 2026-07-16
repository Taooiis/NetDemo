using Microsoft.EntityFrameworkCore;
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

// 自动创建指定实体缺失的表（不指定的不处理，新增实体类型加在这里即可）
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.CreateMissingTables(new List<Type> { 
    //    typeof(MyService.Domain.Entities.TreeNode)
    });
}

// 输出本机局域网 IP 和端口，方便局域网访问
var localIp = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName())
    .AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
if (localIp is not null)
{
    var urls = new[] { "https://0.0.0.0:7256", "http://0.0.0.0:5049" };
    foreach (var url in urls)
    {
        var uri = new Uri(url);
        Console.WriteLine($"局域网访问: {url.Replace("0.0.0.0", localIp.ToString())}");
    }
}

app.Run();