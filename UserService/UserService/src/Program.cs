using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Models;
using UserService.Repositories;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

var necessaryConfigs = new List<string>
    { "Jwt:Issuer", "Jwt:Audience", "Jwt:SecretKey" };
foreach (var necessaryConfig in necessaryConfigs)
    if (string.IsNullOrWhiteSpace(builder.Configuration[necessaryConfig]))
        throw new InvalidOperationException(necessaryConfig + " not found");

builder.Services.AddControllers();
var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
if (postgresConnectionString == null)
    throw new InvalidOperationException("PostgresConnectionString not found");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(postgresConnectionString).UseSnakeCaseNamingConvention();
});
builder.Services.AddScoped<IUserRepository>(provider =>
{
    var passengers = provider.GetRequiredService<ApplicationDbContext>().Set<PassengerProfile>();
    var drivers = provider.GetRequiredService<ApplicationDbContext>().Set<DriverProfile>();
    return new UserRepository(passengers, drivers);
});
builder.Services.AddScoped<ICarRepository>(provider =>
    new CarRepository(provider.GetRequiredService<ApplicationDbContext>().Set<Car>()));
builder.Services.AddScoped<IReviewRepository>(provider =>
    new ReviewRepository(provider.GetRequiredService<ApplicationDbContext>().Set<Review>()));

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// создаем настройки jwt токена
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.OpenConnectionAsync();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
