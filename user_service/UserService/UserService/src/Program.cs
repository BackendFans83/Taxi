using UserService.Repositories;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

var app = builder.Build();

app.MapControllers();

app.Run();