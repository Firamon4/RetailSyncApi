using Microsoft.EntityFrameworkCore;
using RetailSyncApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Визначаємо повний шлях до папки, де лежить програма
var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sync_db.sqlite");

// 1. Підключаємо базу даних SQLite з ПОВНИМ шляхом
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Автоматичне створення бази при запуску
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// 3. Вмикаємо Swagger (щоб працював і на сервері)
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

// Простий Middleware для перевірки ключа
app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    // Пропускаємо перевірку для Swagger та Ping
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