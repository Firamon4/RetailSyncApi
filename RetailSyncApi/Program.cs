using Microsoft.EntityFrameworkCore;
using RetailSyncApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// === ЗМІНИ ТУТ ===
var connectionString = "Server=localhost\\SQLEXPRESS;Database=POSTradeDB;Trusted_Connection=True;TrustServerCertificate=True;";

// Підключаємо SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// =================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Автоматичне створення таблиць у базі (якщо база порожня)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

// Middleware перевірки ключа (без змін)
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (path.StartsWithSegments("/swagger") || path.StartsWithSegments("/api/sync/ping"))
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API Key was not provided");
        return;
    }

    var apiKey = app.Configuration.GetValue<string>("ApiKey") ?? "SecretKey123";

    if (!apiKey.Equals(extractedApiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized client");
        return;
    }

    await next();
});

app.MapControllers();

app.Run();