using UserService.Models;

namespace UserService.Repositories;

public interface IUserRepository
{
    Task CreatePassengerProfileAsync(PassengerProfile profile);
    Task CreateDriverProfileAsync(DriverProfile profile);
}
