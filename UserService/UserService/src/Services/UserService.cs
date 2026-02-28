using UserService.DTOs;
using UserService.Enums;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<bool> CreateUser(CreateUserDto user)
    {
        if (!Enum.TryParse<Role>(user.Role, ignoreCase: true, out var role))
            return false;

        switch (role)
        {
            case Role.Passenger:
            case Role.Admin:
                var passengerProfile = new PassengerProfile(user.Id, user.Name);
                await userRepository.CreatePassengerProfileAsync(passengerProfile);
                return true;

            case Role.Driver:
                var driverProfile = new DriverProfile(user.Id, user.Name);
                await userRepository.CreateDriverProfileAsync(driverProfile);
                return true;

            default:
                return false;
        }
    }
}
