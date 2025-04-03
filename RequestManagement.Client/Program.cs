using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Client.Services;
using RequestManagement.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Добавление MVC
builder.Services.AddControllersWithViews();

// Настройка аутентификации с использованием cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         // Путь для входа
        options.AccessDeniedPath = "/Account/AccessDenied"; // Путь при отказе в доступе
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Время жизни cookie совпадает с JWT
    });

// Настройка сессии для хранения JWT-токена
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Регистрация gRPC-клиентов
builder.Services.AddGrpcClient<RequestService.RequestServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001"); // Адрес сервера
});

builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001"); // Адрес сервера
});

// Регистрация сервисов
builder.Services.AddScoped<GrpcRequestService>();
builder.Services.AddScoped<GrpcAuthService>();
builder.Services.AddHttpContextAccessor(); // Для доступа к HttpContext в сервисах

// Добавление логирования
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

// Настройка конвейера обработки запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Middleware для проверки cookie
app.UseAuthorization();  // Middleware для проверки прав доступа
app.UseSession();        // Middleware для работы с сессией

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();