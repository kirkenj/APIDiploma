using Database.Interfaces;
using Database;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using API.Mapping;
using Web.Middlewares;
using static Web.Constants.IncludeModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 33))));
builder.Services.AddTransient<IAppDBContext, AppDbContext>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IContractService, ContractService>();
builder.Services.AddTransient<IHashProvider, HashProvider>((a) => new HashProvider(HashAlgorithm.Create("MD5") ?? throw new ArgumentException("Hash algorithm not found"), System.Text.Encoding.UTF8));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNavigation.OnlyAdminPolicy.PolicyName, policy =>
        policy.RequireRole(PolicyNavigation.OnlyAdminPolicy.RoleNames));
    options.AddPolicy(PolicyNavigation.OnlySuperAdminPolicy.PolicyName, policy =>
        policy.RequireRole(PolicyNavigation.OnlySuperAdminPolicy.RoleNames));
});

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
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
