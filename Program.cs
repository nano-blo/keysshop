using KeysShop.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

/*var builder = WebApplication.CreateBuilder(args);
*/


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Конфигурация БД
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=dpg-d5chq9ili9vc73cibbdg-a;Port=5432;Database=keysshopdb;Username=keysshopdb_user;Password=TXFgDCd1SnDxmEOnjTrR6RZCXSLzjkzq;SslMode=Require";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();

// Подключение к БД
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
/*builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
*/

// Если строка пустая - используем тестовую
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Host=localhost;Database=keysshop;Username=postgres;Password=postgres";
    Console.WriteLine("⚠️ ВНИМАНИЕ: Используется тестовая строка подключения");
}
else
{
    Console.WriteLine($"✅ Строка подключения получена, длина: {connectionString.Length}");
}

// Простая и надежная настройка DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });
});

var app = builder.Build();

// АВТОМАТИЧЕСКОЕ СОЗДАНИЕ БД при запуске
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Создаем базу данных если её нет
        dbContext.Database.EnsureCreated();

        // ИЛИ применить миграции
        // dbContext.Database.Migrate();

        Console.WriteLine("✅ База данных создана успешно!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка создания БД: {ex.Message}");
    }
}

app.UseSession();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalog}/{action=Index}/{id?}");

app.Urls.Add("http://0.0.0.0:" + (Environment.GetEnvironmentVariable("PORT") ?? "5000"));

// Простой health check
app.MapGet("/health", () =>
{
    var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    return Results.Json(new
    {
        status = "running",
        timestamp = DateTime.UtcNow,
        database_configured = dbUrl != null,
        database_url_length = dbUrl?.Length ?? 0,
        environment = builder.Environment.EnvironmentName
    });
});

// Простая главная страница
app.MapGet("/", () => "🚀 KeysShop запущен! Проверьте /health для статуса");

app.Run();