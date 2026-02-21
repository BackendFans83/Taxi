using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    DbSet<Car> Cars { get; set; }
    DbSet<Review> Review { get; set; }
    DbSet<PassengerProfile> PassengerProfiles { get; set; }
    DbSet<DriverProfile> DriverProfiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Car>(cars =>
        {
            cars.ToTable("cars");
            cars.HasKey(c => c.Id);
        });
        
        builder.Entity<Review>(reviews =>
        {
            reviews.ToTable("reviews");
            reviews.HasKey(r => r.Id);
        });
        
        builder.Entity<PassengerProfile>(passengerProfiles =>
        {
            passengerProfiles.ToTable("passenger_profiles");
            passengerProfiles.HasKey(pp => pp.Id);
        });
        
        builder.Entity<DriverProfile>(driverProfiles =>
        {
            driverProfiles.ToTable("driver_profiles");
            driverProfiles.HasKey(dp => dp.Id);
        });
    }
}