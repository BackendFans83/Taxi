using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RideService.Data;
using RideService.Producers;
using RideService.Repositories;
using RideService.Services;

var builder = WebApplication.CreateBuilder(args);

var necessaryConfigs = new List<string>
{
    "Jwt:Issuer", "Jwt:Audience", "Jwt:SecretKey", "Kafka:BootstrapServers", "Kafka:GroupId", "Kafka:RideTopic",
    "Kafka:RideCreatedEvent"
};
foreach (var necessaryConfig in necessaryConfigs)
    if (string.IsNullOrWhiteSpace(builder.Configuration[necessaryConfig]))
        throw new InvalidOperationException(necessaryConfig + " not found");

builder.Services.AddControllers();
builder.Services.AddScoped<IRideService, RideService.Services.RideService>();
builder.Services.AddScoped<IRideRepository, RideRepository>();
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

#region db_connections

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
if (postgresConnectionString == null)
    throw new InvalidOperationException("PostgresConnectionString not found");
builder.Services.AddDbContext<DbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(postgresConnectionString).UseSnakeCaseNamingConvention();
});

#endregion

#region auth

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

#endregion

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.OpenConnectionAsync();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();