using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class NotificationsDbContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; }

    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}