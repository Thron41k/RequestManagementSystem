using Microsoft.EntityFrameworkCore;
using RequestManagement.Server.Data;
using RequestManagement.Server.Services;
using RequestManagement.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Регистрация контекста БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация сервисов
builder.Services.AddScoped<INomenclatureService, NomenclatureService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomingService, IncomingService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ICommissionsService, CommissionsService>();
builder.Services.AddScoped<INomenclatureAnalogService, NomenclatureAnalogService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IDefectGroupService, DefectGroupService>();
builder.Services.AddScoped<IDefectService, DefectService>();

// Настройка JWT
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

// Добавление gRPC
builder.Services.AddGrpc();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Настройка конвейера обработки запросов
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<RequestManagement.Server.Controllers.NomenclatureController>();
app.MapGrpcService<RequestManagement.Server.Controllers.StockController>();
app.MapGrpcService<RequestManagement.Server.Controllers.AuthController>();
app.MapGrpcService<RequestManagement.Server.Controllers.ExpenseController>();
app.MapGrpcService<RequestManagement.Server.Controllers.IncomingController>();
app.MapGrpcService<RequestManagement.Server.Controllers.WarehouseController>();
app.MapGrpcService<RequestManagement.Server.Controllers.CommissionsController>();
app.MapGrpcService<RequestManagement.Server.Controllers.NomenclatureAnalogController>();
app.MapGrpcService<RequestManagement.Server.Controllers.EquipmentController>();
app.MapGrpcService<RequestManagement.Server.Controllers.DriverController>();
app.MapGrpcService<RequestManagement.Server.Controllers.DefectController>();
app.MapGrpcService<RequestManagement.Server.Controllers.DefectGroupController>();

app.Run();