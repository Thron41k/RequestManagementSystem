using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Client.Services;
using RequestManagement.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ���������� MVC
builder.Services.AddControllersWithViews();

// ��������� �������������� � �������������� cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         // ���� ��� �����
        options.AccessDeniedPath = "/Account/AccessDenied"; // ���� ��� ������ � �������
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // ����� ����� cookie ��������� � JWT
    });

// ��������� ������ ��� �������� JWT-������
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ����������� gRPC-��������
builder.Services.AddGrpcClient<RequestService.RequestServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001"); // ����� �������
});

builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001"); // ����� �������
});

// ����������� ��������
builder.Services.AddScoped<GrpcRequestService>();
builder.Services.AddScoped<GrpcAuthService>();
builder.Services.AddHttpContextAccessor(); // ��� ������� � HttpContext � ��������

// ���������� �����������
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

// ��������� ��������� ��������� ��������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Middleware ��� �������� cookie
app.UseAuthorization();  // Middleware ��� �������� ���� �������
app.UseSession();        // Middleware ��� ������ � �������

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();