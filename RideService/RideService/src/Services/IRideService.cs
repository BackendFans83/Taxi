using RideService.DTOs;
using RideService.Models;

namespace RideService.Services;

public interface IRideService
{
    Task<Result<Ride>> CreateRideAsync(CreateRideDto dto);
}