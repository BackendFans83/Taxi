using AuthService.Clients;
using AuthService.Data;
using AuthService.Repositories;
using AuthService.Services;
using AuthService.Utils;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrEmpty(builder.Configuration["Jwt:SecretKey"]))
    throw new InvalidOperationException("Jwt:SecretKey not found");
if (string.IsNullOrEmpty(builder.Configuration["Client:Url"]))
    throw new InvalidOperationException("Client:Url not found");

builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddScoped<IOAuthService, OAuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ICacheRepository, RedisCacheRepository>();
builder.Services.AddScoped<IAccessTokenGenerator, JwtAccessTokenGenerator>();
builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();

builder.Services.AddHttpClient<GoogleOAuthClient>();
builder.Services.AddHttpClient<AppleOAuthClient>();
builder.Services.AddScoped<IOAuthClient>(sp => sp.GetRequiredService<GoogleOAuthClient>());
builder.Services.AddScoped<IOAuthClient>(sp => sp.GetRequiredService<AppleOAuthClient>());

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
if (postgresConnectionString == null)
    throw new InvalidOperationException("PostgresConnectionString not found");
builder.Services.AddDbContext<DbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(postgresConnectionString).UseSnakeCaseNamingConvention();
});

var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnectionString");
if (redisConnectionString == null)
    throw new InvalidOperationException("RedisConnectionString not found");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(p =>
    p.WithOrigins(builder.Configuration["Client:Url"]!).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.MapControllers();

app.Run();