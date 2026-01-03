using Microsoft.EntityFrameworkCore;
using KeysShop.Data;

var builder = WebApplication.CreateBuilder(args);

// Строка подключения к PostgreSQL Render
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// АВТОМАТИЧЕСКОЕ СОЗДАНИЕ БАЗЫ ДАННЫХ
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        Console.WriteLine("🚀 Начинаю создание базы данных...");

        var context = services.GetRequiredService<AppDbContext>();

        // Создаем базу и таблицы если их нет
        context.Database.EnsureCreated();

        Console.WriteLine("✅ База данных создана/проверена успешно!");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка при создании базы данных: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("🎉 Приложение готово к работе!");
app.Run();