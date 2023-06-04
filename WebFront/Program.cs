using ClosedXML.Excel;
using ClosedXML.Graphics;
using Database;
using Database.Interfaces;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebFront.Mapping;
using WebFront.Middlewares;
using static WebFront.Constants.IncludeModels;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();



//builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(Environment.GetEnvironmentVariable("DiplomaLocalMySQLConnectionString") ?? throw new Exception($"DiplomaLocalMySQLConnectionString not found'"), new MySqlServerVersion(new Version(8, 0, 33))));
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(Environment.GetEnvironmentVariable("DiplomaDatabaseConnectionString") ?? throw new Exception($"DiplomaDatabaseConnectionString not found'"), new MySqlServerVersion(new Version(8, 0, 33))));

LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine("Times New Roman");
builder.Services.AddTransient<IAppDBContext, AppDbContext>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IContractService, ContractService>();
builder.Services.AddTransient<IContractTypeService, ContractTypeService>();
builder.Services.AddTransient<IContractLinkingPartService, ContractLinkingPartService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IExcelReportService, ExcelReportService>();
builder.Services.AddTransient<IMonthReportService, MonthReportService>();
builder.Services.AddTransient<IAcademicDegreeService, AcademicDegreeService>();
builder.Services.AddTransient<IHashProvider, HashProvider>((a) => new HashProvider(HashAlgorithm.Create("MD5") ?? throw new ArgumentException("Hash algorithm not found"), System.Text.Encoding.UTF8));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNavigation.OnlyAdminPolicy.PolicyName, policy =>
        policy.RequireRole(PolicyNavigation.OnlyAdminPolicy.RoleNames));
    options.AddPolicy(PolicyNavigation.OnlySuperAdminPolicy.PolicyName, policy =>
        policy.RequireRole(PolicyNavigation.OnlySuperAdminPolicy.RoleNames));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();