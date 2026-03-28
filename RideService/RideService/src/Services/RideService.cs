using RideService.DTOs;
using RideService.Models;
using RideService.Producers;
using RideService.Repositories;

namespace RideService.Services;

public class RideService(IRideRepository rideRepository, IKafkaProducer kafkaProducer) : IRideService
{
    public async Task<Result<RideDto>> CreateRideAsync(CreateRideDto dto)
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
        Ride created;

        try
        {
            created = await rideRepository.AddAsync(ride);
            await kafkaProducer.SendRideCreatedEventAsync(new RideDto(created));
        }
        catch (Exception ex)
        {
            return Result<RideDto>.Failure(500, ex.Message);
        }

        return Result<RideDto>.Success(new RideDto(created));
    }
}