using ClosedXML.Excel;
using ClosedXML.Graphics;
using Database;
using Database.Interfaces;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using WebFront.Mapping;
using WebFront.Middlewares;
using WebFront.Models.JWTModels;
using static WebFront.Constants.IncludeModels;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
var secretKey = builder.Configuration.GetSection("JWTSettings:SecretKey").Value ?? throw new Exception("builder.Configuration.GetSection(\"JWTSettings:SecretKey\").Value is null");
var issuer = builder.Configuration.GetSection("JWTSettings:Issuer").Value ?? throw new Exception("builder.Configuration.GetSection(\"JWTSettings:Issuer\").Value is null");
var audience = builder.Configuration.GetSection("JWTSettings:Audience").Value ?? throw new Exception("builder.Configuration.GetSection(\"JWTSettings:Audience\").Value is null");
var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
var settings = new JWTSettings() { Audience = audience, Issuer = issuer, SecretKey = secretKey };
builder.Services.AddSingleton(settings);


//builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(Environment.GetEnvironmentVariable("DiplomaLocalMySQLConnectionString") ?? throw new Exception($"DiplomaLocalMySQLConnectionString not found'"), new MySqlServerVersion(new Version(8, 0, 33))));
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql("server=localhost;user=root;password=12345678;database=contracts_local;", new MySqlServerVersion(new Version(8, 0, 33))));

LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine("Times New Roman");

builder.Services.AddTransient<IAppDBContext, AppDbContext>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IContractService, ContractService>();
builder.Services.AddTransient<IContractTypeService, ContractTypeService>();
builder.Services.AddTransient<IContractLinkingPartService, ContractLinkingPartService>();
builder.Services.AddTransient<IExcelReportService, ExcelReportService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IMonthReportService, MonthReportService>();
builder.Services.AddTransient<IAcademicDegreeService, AcademicDegreeService>();
builder.Services.AddTransient<IHashProvider, HashProvider>((a) => new HashProvider(HashAlgorithm.Create("MD5") ?? throw new ArgumentException("Hash algorithm not found"), System.Text.Encoding.UTF8));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNavigation.OnlyAdminPolicy.PolicyName, policy =>
        policy.RequireRole(PolicyNavigation.OnlyAdminPolicy.RoleNames));
    options.AddPolicy(PolicyNavigation.OnlySuperAdminPolicy.PolicyName, policy =>
        policy.RequireRole(PolicyNavigation.OnlySuperAdminPolicy.RoleNames));
});

var app = builder.Build();

app.UseCors(builder => builder.WithOrigins("http://web.infinity-movies.ru/").AllowAnyOrigin().AllowAnyMethod().WithHeaders("Content-Type").AllowAnyHeader());
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

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();
