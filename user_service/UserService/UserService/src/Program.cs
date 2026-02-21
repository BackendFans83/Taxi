using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.Repositories;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db=scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.OpenConnectionAsync();
app.MapControllers();

app.Run();