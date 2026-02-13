using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Credentials> Credentials { get; set; }
}