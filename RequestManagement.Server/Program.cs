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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ��������� JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]); // ���� ������ ���� ������� 32 �����
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

// ��������� ��������� ��������� ��������
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<RequestManagement.Server.Controllers.RequestController>();
app.MapGrpcService<RequestManagement.Server.Controllers.AuthController>();

app.Run();