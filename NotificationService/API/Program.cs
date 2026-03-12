using API.ExtensionMethods;
using Domain.Enums;
using DotNetEnv;
using Infrastructure;
using Infrastructure.Entities;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Env.Load();
        builder.Services.AddNotificationsDbContext();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.Run();
    }
}