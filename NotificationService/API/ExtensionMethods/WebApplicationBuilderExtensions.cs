using DotNetEnv;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.ExtensionMethods;

public static class WebApplicationBuilderExtensions
{
    public static void AddNotificationsDbContext(this IServiceCollection services)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
        {
            options.UseNpgsql(Env.GetString("POSTGRES_CONNECTION_STRING"));
        });
    }
}