using Microsoft.EntityFrameworkCore;
using RideService.Data;
using RideService.Repositories;
using RideService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRideService, RideService.Services.RideService>();
builder.Services.AddScoped<IRideRepository, RideRepository>();

#region db_connections

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
if (postgresConnectionString == null)
    throw new InvalidOperationException("PostgresConnectionString not found");
builder.Services.AddDbContext<DbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(postgresConnectionString).UseSnakeCaseNamingConvention();
});

#endregion

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.OpenConnectionAsync();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();