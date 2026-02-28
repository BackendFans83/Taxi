using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Review> Review { get; set; }
    public DbSet<PassengerProfile> PassengerProfiles { get; set; }
    public DbSet<DriverProfile> DriverProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Car>(cars =>
        {
            cars.ToTable("cars");
            cars.HasKey(c => c.Id);
            cars.HasIndex(c => c.DriverId);
            cars.HasIndex(c => c.Number).IsUnique();
        });

        builder.Entity<Review>(reviews =>
        {
            reviews.ToTable("reviews");
            reviews.HasKey(r => r.Id);
            reviews.HasIndex(r => r.AuthorId);
            reviews.HasIndex(r => r.RecipientId);
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
            driverProfiles.HasIndex(dp => dp.LicenseNumber).IsUnique();
        });
    }
}