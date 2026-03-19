using RideService.Models;

namespace RideService.Repositories;

public interface IRideRepository
{
    Task<Ride> AddAsync(Ride ride);
}