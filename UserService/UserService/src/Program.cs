using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using UserService.Consumers;
using UserService.Data;
using UserService.Repositories;
using UserService.Services;
using UserService.Utils;

var builder = WebApplication.CreateBuilder(args);

var necessaryConfigs = new List<string>
    { "Jwt:Issuer", "Jwt:Audience", "Jwt:SecretKey", "Kafka:BootstrapServers", "Kafka:Topic", "Kafka:GroupId" };
foreach (var necessaryConfig in necessaryConfigs)
    if (string.IsNullOrWhiteSpace(builder.Configuration[necessaryConfig]))
        throw new InvalidOperationException(necessaryConfig + " not found");

builder.Services.AddControllers();

#region db

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
if (postgresConnectionString == null)
    throw new InvalidOperationException("PostgresConnectionString not found");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(postgresConnectionString).UseSnakeCaseNamingConvention();
});

#endregion

#region services DI

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddHostedService<KafkaConsumer>();

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

#region swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Taxi User Service",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });
    c.OperationFilter<OneOfSchemaFilter>();
});

#endregion

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.OpenConnectionAsync();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Taxi User Service"); });
}

app.Run();