using Microsoft.EntityFrameworkCore;
using RequestManagement.Server.Data;
using RequestManagement.Server.Services;
using RequestManagement.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


//// Регистрация контекста БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
//        .LogTo(Console.WriteLine, LogLevel.Information) // вывод всех SQL в консоль
//        .EnableSensitiveDataLogging() // покажет реальные параметры
//        .EnableDetailedErrors());

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
builder.Services.AddScoped<IEquipmentGroupService, EquipmentGroupService>();
builder.Services.AddScoped<ISparePartsOwnershipService, SparePartsOwnershipService>();
builder.Services.AddScoped<IMaterialsInUseService, MaterialsInUseService>();
builder.Services.AddScoped<IReasonsForWritingOffMaterialsFromOperationService, ReasonsForWritingOffMaterialsFromOperationService>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

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

builder.Services.AddGrpc();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseRouting();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<RequestManagement.Server.Controllers.NomenclatureController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.NomenclatureController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.StockController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.StockController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.AuthController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.AuthController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.ExpenseController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.ExpenseController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.IncomingController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.IncomingController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.WarehouseController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.WarehouseController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.CommissionsController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.CommissionsController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.NomenclatureAnalogController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.NomenclatureAnalogController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.EquipmentController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.EquipmentController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.DriverController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.DriverController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.DefectController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.DefectController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.DefectGroupController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.DefectGroupController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.EquipmentGroupController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.EquipmentGroupController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.SparePartsOwnershipController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.SparePartsOwnershipController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.MaterialsInUseController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.MaterialsInUseController>().RequireHost("*:5000");
app.MapGrpcService<RequestManagement.Server.Controllers.ReasonsForWritingOffMaterialsFromOperationController>().EnableGrpcWeb().RequireHost("*:5001");
app.MapGrpcService<RequestManagement.Server.Controllers.ReasonsForWritingOffMaterialsFromOperationController>().RequireHost("*:5000");

app.Run();