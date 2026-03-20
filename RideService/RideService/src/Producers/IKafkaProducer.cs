using RideService.DTOs;
using RideService.Models;

namespace RideService.Producers;

public interface IKafkaProducer
{
    Task<Result> SendRideCreatedEventAsync(RideDto dto);
}