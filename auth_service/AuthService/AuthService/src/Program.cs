using AuthService.Clients;
using AuthService.Repositories;
using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService,AuthService.Services.AuthService>();
builder.Services.AddScoped<IOAuthService,OAuthService>();
builder.Services.AddScoped<IAuthRepository,AuthRepository>();
builder.Services.AddScoped<ICacheRepository,RedisCacheRepository>();

builder.Services.AddHttpClient<GoogleOAuthClient>();
builder.Services.AddHttpClient<AppleOAuthClient>();
builder.Services.AddScoped<IOAuthClient>(sp=>sp.GetRequiredService<GoogleOAuthClient>());
builder.Services.AddScoped<IOAuthClient>(sp=>sp.GetRequiredService<AppleOAuthClient>());

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(p=>p.WithOrigins(builder.Configuration["Client:Url"]).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.MapControllers();

app.Run();