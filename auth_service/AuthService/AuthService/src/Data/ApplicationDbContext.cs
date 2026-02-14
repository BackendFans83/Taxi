using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Credentials> Credentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credentials>(credentials =>
        {
            credentials.ToTable("credentials");
            credentials.HasKey(c => c.Id);
            
            credentials.HasIndex(c => c.Email).IsUnique();
            credentials.HasIndex(c => c.GoogleOAuthId).IsUnique();
            credentials.HasIndex(c => c.AppleOAuthId).IsUnique();
        });
    }
}