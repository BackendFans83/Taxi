using RideService.Data;
using RideService.Models;

namespace RideService.Repositories;

public class RideRepository(ApplicationDbContext dbContext) : IRideRepository
{
    public async Task<Ride> AddAsync(Ride ride)
    {
        dbContext.Rides.Add(ride);
        await dbContext.SaveChangesAsync();
        return ride;
    }
}