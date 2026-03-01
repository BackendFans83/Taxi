using Microsoft.EntityFrameworkCore;
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

    public async Task<PassengerProfile?> GetPassengerByIdAsync(int id)
        => await dbContext.PassengerProfiles.FindAsync(id);

    public async Task<DriverProfile?> GetDriverByIdAsync(int id)
        => await dbContext.DriverProfiles.FindAsync(id);

    public Task UpdatePassengerAsync(PassengerProfile profile)
    {
        dbContext.PassengerProfiles.Update(profile);
        return dbContext.SaveChangesAsync();
    }

    public Task UpdateDriverAsync(DriverProfile profile)
    {
        dbContext.DriverProfiles.Update(profile);
        return dbContext.SaveChangesAsync();
    }
}
