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
using Web.Models.JWTModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


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



//builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql("server=icrafts.beget.tech;user=icrafts_test;password=prB%cnJ5;database=icrafts_test;", new MySqlServerVersion(new Version(8, 0, 33))));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(Environment.GetEnvironmentVariable("DiplomaLocalMySQLConnectionString") ?? throw new Exception($"DiplomaLocalMySQLConnectionString not found'"), new MySqlServerVersion(new Version(8, 0, 33))));
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(Environment.GetEnvironmentVariable("DiplomaDatabaseConnectionString") ?? throw new Exception($"DiplomaDatabaseConnectionString not found'"), new MySqlServerVersion(new Version(8, 0, 33))));
builder.Services.AddTransient<IAppDBContext, AppDbContext>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<IContractService, ContractService>();
builder.Services.AddTransient<IContractTypeService, ContractTypeService>();
builder.Services.AddTransient<IContractLinkingPartService, ContractLinkingPartService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IMonthReportService, MonthReportService>();
builder.Services.AddTransient<IAcademicDegreeService, AcademicDegreeService>();
builder.Services.AddTransient<IHashProvider, HashProvider>((a) => new HashProvider(HashAlgorithm.Create("MD5") ?? throw new ArgumentException("Hash algorithm not found"), System.Text.Encoding.UTF8));

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options => options.LoginPath = "/login");

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

app.UseCors(builder => builder.AllowAnyOrigin());
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
