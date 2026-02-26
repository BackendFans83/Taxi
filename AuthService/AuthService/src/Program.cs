using System.Text;
using AuthService.Clients;
using AuthService.Data;
using AuthService.Repositories;
using AuthService.Services;
using AuthService.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrEmpty(builder.Configuration["Jwt:Issuer"]))
    throw new InvalidOperationException("Jwt:Issuer not found");
if (string.IsNullOrEmpty(builder.Configuration["Jwt:Audience"]))
    throw new InvalidOperationException("Jwt:Audience not found");
if (string.IsNullOrEmpty(builder.Configuration["Jwt:SecretKey"]))
    throw new InvalidOperationException("Jwt:SecretKey not found");
if (string.IsNullOrEmpty(builder.Configuration["Client:Url"]))
    throw new InvalidOperationException("Client:Url not found");

#region services_DI
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
#endregion

#region db_connections
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
#endregion

#region auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
    };
});
builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(p =>
    p.WithOrigins(builder.Configuration["Client:Url"]!).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();