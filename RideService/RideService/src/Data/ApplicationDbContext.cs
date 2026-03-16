using Microsoft.EntityFrameworkCore;
using RideService.Models;

namespace RideService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    DbSet<Ride> Rides { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ride>(ride =>
        {
            ride.ToTable("rides");
            ride.HasKey(r => r.Id);
            
            ride.HasIndex(r => r.DriverId);
            ride.HasIndex(r => r.PassengerId);
        });
    }
}