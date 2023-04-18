using Database.Interfaces;
using Database;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using API.Mapping;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IAppDBContext, AppDbContext>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IContractService, ContractService>();
builder.Services.AddTransient<IMonthReportService, MonthReportService>();

builder.Services.AddTransient<IHashProvider, HashProvider>((a) => new HashProvider(HashAlgorithm.Create("MD5") ?? throw new ArgumentException("Hash algorithm not found"), System.Text.Encoding.UTF8));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdmin", policy =>
        policy.RequireRole(new[] { "Admin", "SuperAdmin" }));
    options.AddPolicy("OnlySuperAdmin", policy =>
        policy.RequireRole(new[] { "SuperAdmin" }));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
