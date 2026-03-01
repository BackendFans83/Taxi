using UserService.DTOs;

namespace UserService.Services;

public interface IUserService
{
    Task<Result> CreateUser(CreateUserDto user);
    Task<Result<PassengerProfileDto>> GetPassengerProfileAsync(int id);
    Task<Result<DriverProfileDto>> GetDriverProfileAsync(int id);
    Task<Result<PassengerProfileDto>> UpdatePassengerProfileAsync(int id, UpdatePassengerProfileRequest request);
    Task<Result<DriverProfileDto>> UpdateDriverProfileAsync(int id, UpdateDriverProfileRequest request);
}
