using UserService.DTOs;
using UserService.Enums;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<Result> CreateUser(CreateUserDto user)
    {
        if (!Enum.TryParse<Role>(user.Role, ignoreCase: true, out var role))
            return Result.Failure(400, "Invalid role");

        switch (role)
        {
            case Role.Passenger:
            case Role.Admin:
                var passengerProfile = new PassengerProfile(user.Id, user.Name);
                await userRepository.CreatePassengerProfileAsync(passengerProfile);
                return Result.Success();

            case Role.Driver:
                var driverProfile = new DriverProfile(user.Id, user.Name);
                await userRepository.CreateDriverProfileAsync(driverProfile);
                return Result.Success();

            default:
                return Result.Failure(400, "Unknown role");
        }
    }

    public async Task<Result<PassengerProfileDto>> GetPassengerProfileAsync(int id)
    {
        var profile = await userRepository.GetPassengerByIdAsync(id);
        return profile == null
            ? Result<PassengerProfileDto>.Failure(404, "Passenger profile not found")
            : Result<PassengerProfileDto>.Success(new PassengerProfileDto(profile));
    }

    public async Task<Result<DriverProfileDto>> GetDriverProfileAsync(int id)
    {
        var profile = await userRepository.GetDriverByIdAsync(id);
        return profile == null
            ? Result<DriverProfileDto>.Failure(404, "Driver profile not found")
            : Result<DriverProfileDto>.Success(new DriverProfileDto(profile));
    }

    public async Task<Result<PassengerProfileDto>> UpdatePassengerProfileAsync(int id, UpdatePassengerProfileRequest request)
    {
        var profile = await userRepository.GetPassengerByIdAsync(id);
        if (profile == null)
            return Result<PassengerProfileDto>.Failure(404, "Passenger profile not found");

        profile.Name = request.Name;
        profile.AvatarUrl = request.AvatarUrl;

        await userRepository.UpdatePassengerAsync(profile);
        return Result<PassengerProfileDto>.Success(new PassengerProfileDto(profile));
    }

    public async Task<Result<DriverProfileDto>> UpdateDriverProfileAsync(int id, UpdateDriverProfileRequest request)
    {
        var profile = await userRepository.GetDriverByIdAsync(id);
        if (profile == null)
            return Result<DriverProfileDto>.Failure(404, "Driver profile not found");

        profile.Name = request.Name;
        profile.AvatarUrl = request.AvatarUrl;
        profile.LicenseNumber = request.LicenseNumber ?? string.Empty;
        profile.LicenseExpiryDate = request.LicenseExpiryDate;
        profile.CurrentCarId = request.CurrentCarId;

        await userRepository.UpdateDriverAsync(profile);
        return Result<DriverProfileDto>.Success(new DriverProfileDto(profile));
    }
}
