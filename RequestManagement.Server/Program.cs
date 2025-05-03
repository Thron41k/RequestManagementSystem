using Microsoft.EntityFrameworkCore;
using RequestManagement.Server.Data;
using RequestManagement.Server.Services;
using RequestManagement.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// ����������� ��������� ��
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� ��������
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomingService, IncomingService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ICommissionsService, CommissionsService>();

// ��������� JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException());
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// ���������� gRPC
builder.Services.AddGrpc();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// ��������� ��������� ��������� ��������
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<RequestManagement.Server.Controllers.RequestController>();
app.MapGrpcService<RequestManagement.Server.Controllers.StockController>();
app.MapGrpcService<RequestManagement.Server.Controllers.AuthController>();
app.MapGrpcService<RequestManagement.Server.Controllers.ExpenseController>();
app.MapGrpcService<RequestManagement.Server.Controllers.IncomingController>();
app.MapGrpcService<RequestManagement.Server.Controllers.WarehouseController>();
app.MapGrpcService<RequestManagement.Server.Controllers.CommissionsController>();

app.Run();