using UserService.Data;
using UserService.Models;

namespace UserService.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public async Task CreatePassengerProfileAsync(PassengerProfile profile)
    {
        dbContext.PassengerProfiles.Add(profile);
        await dbContext.SaveChangesAsync();
    }

    public async Task CreateDriverProfileAsync(DriverProfile profile)
    {
        dbContext.DriverProfiles.Add(profile);
        await dbContext.SaveChangesAsync();
    }
}
