using AuthService.DTOs;

namespace AuthService.Producers;

public interface IKafkaProducer
{
    Task<Result> SendUserRegisteredEventAsync(CreateUserDto dto);
}