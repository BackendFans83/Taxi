using UserService.Models;

namespace UserService.Repositories;

public interface IUserRepository
{
    Task<bool> CreatePassengerProfileAsync(PassengerProfile profile);
    Task<bool> CreateDriverProfileAsync(DriverProfile profile);
    Task<PassengerProfile?> GetPassengerByIdAsync(int id);
    Task<DriverProfile?> GetDriverByIdAsync(int id);
    Task UpdatePassengerAsync(PassengerProfile profile);
    Task UpdateDriverAsync(DriverProfile profile);
}
