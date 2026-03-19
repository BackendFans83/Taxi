using RideService.DTOs;
using RideService.Models;
using RideService.Repositories;

namespace RideService.Services;

public class RideService(IRideRepository rideRepository) : IRideService
{
    public async Task<Result<Ride>> CreateRideAsync(CreateRideDto dto)
    {
        var ride = new Ride
        {
            PassengerId = dto.PassengerId,
            PickupLatitude = dto.PickupLatitude,
            PickupLongitude = dto.PickupLongitude,
            DropOffLatitude = dto.DropOffLatitude,
            DropOffLongitude = dto.DropOffLongitude,
            PickupAddress = dto.PickupAddress,
            DropOffAddress = dto.DropOffAddress,
            Status = RideStatus.Requested,
            RequestedAt = DateTime.UtcNow
        };

        var created = await rideRepository.AddAsync(ride);
        return Result<Ride>.Success(created);
    }
}