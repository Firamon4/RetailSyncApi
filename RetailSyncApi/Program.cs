using Microsoft.EntityFrameworkCore;
using RetailSyncApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Підключаємо базу даних SQLite (файл створиться сам)
// Для продакшену на корпоративному сервері ти просто заміниш це на UseSqlServer
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=sync_db.sqlite"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Зручна адмінка для тестів

var app = builder.Build();

// 2. Автоматичне створення бази при запуску (щоб не гратися з міграціями вручну зараз)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// 3. Вмикаємо Swagger (щоб ти міг тестити прямо в браузері)
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();